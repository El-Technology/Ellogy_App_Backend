using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common;
using AICommunicationService.Common.Dtos;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.RAG.Interfaces;
using AICommunicationService.RAG.Models;
using AutoMapper;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using LangChain.TextSplitters;
using Pgvector;
using System.Data.Common;
using System.Text;
using UglyToad.PdfPig;

namespace AICommunicationService.BLL.Services;

public class DocumentService : IDocumentService
{
    private const int SAS_TTL = 5;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IAzureOpenAiRequestService _customAiService;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentSharingRepository _documentSharingRepository;
    private readonly IEmbeddingRepository _embeddingRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public DocumentService(BlobServiceClient blobServiceClient,
        IAzureOpenAiRequestService customAiService,
        IEmbeddingRepository embeddingRepository,
        IDocumentRepository documentRepository,
        IMapper mapper,
        IUserRepository userRepository,
        IDocumentSharingRepository documentSharingRepository)
    {
        _documentSharingRepository = documentSharingRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _documentRepository = documentRepository;
        _embeddingRepository = embeddingRepository;
        _customAiService = customAiService;
        _blobServiceClient = blobServiceClient;
    }

    /// <inheritdoc cref="IDocumentService.DeleteFileAsync" />
    public async Task DeleteFileAsync(Guid userId, string fileName)
    {
        var document = await _documentRepository.GetDocumentByNameAsync(userId, fileName)
                       ?? throw new ArgumentNullException(nameof(fileName));

        if (userId != document.UserId)
            throw new Exception("You don`t have access to delete this file");

        var httpClient = new HttpClient();
        var response =
            await httpClient.DeleteAsync(ReturnUrlWithPermission(userId, fileName, 5, BlobSasPermissions.Delete));

        if (!response.IsSuccessStatusCode)
            throw new Exception("Delete file error, try again");

        await _embeddingRepository.DeleteEmbeddingsAsync(userId, fileName);
        await _documentRepository.DeleteDocumentAsync(userId, fileName);
        await _documentSharingRepository.DeleteAllDocumentSharingAsync(document.Id);
    }

    /// <inheritdoc cref="IDocumentService.CheckIfDocumentWasUploadedAsync" />
    public async Task CheckIfDocumentWasUploadedAsync(Guid userId, string fileName)
    {
        var blobClient = GetBlobContainerClient(userId, fileName);

        if (!await blobClient.ExistsAsync())
            throw new Exception($"File with name:{fileName} cannot be found, try again later");

        var document = new Document
        {
            Id = Guid.NewGuid(),
            Name = fileName,
            UserId = userId,
            CreationDate = DateTime.UtcNow,
            IsReadyToUse = false
        };

        await _documentRepository.AddDocumentAsync(document);
    }

    /// <inheritdoc cref="IDocumentService.GetAllUserDocumentsAsync" />
    public async Task<PaginationResponseDto<DocumentResponseWithOwner>> GetAllUserDocumentsAsync(Guid userId,
        PaginationRequestDto paginationRequest)
    {
        var documentResponse =
            _mapper.Map<PaginationResponseDto<DocumentResponseWithOwner>>(
                await _documentRepository.GetAllUserDocumentsAsync(userId, paginationRequest));

        var users = await _userRepository.GetUsersByIdsAsync(documentResponse.Data.Select(a => a.UserId).ToList());

        foreach (var document in documentResponse.Data)
        {
            var user = users.FirstOrDefault(a => a.Id == document.UserId);
            document.Email = user.Email;
            document.FirstName = user.FirstName;
            document.LastName = user.LastName;
            document.AvatarLink = user.AvatarLink;
        }

        return documentResponse;
    }

    /// <inheritdoc cref="IDocumentService.InsertDocumentContextInVectorDbAsync" />
    public async Task<DocumentResponseWithOwner> InsertDocumentContextInVectorDbAsync(Guid userId, string fileName)
    {
        var documentIsReady = false;

        var document = await _documentRepository.GetDocumentByNameAsync(userId, fileName)
                       ?? throw new Exception("Document was not found");

        if (await _embeddingRepository.CheckIfEmbeddingAlreadyExistAsync(userId, fileName))
            throw new Exception("Embedding already exist");

        try
        {
            var documentContext = await ReadPdfAsync(userId, fileName);

            var splitText = SplitText(documentContext);
            var embeddings = new List<Embedding>();

            foreach (var sanitizedText in splitText.Select(text => text.Replace("\0", string.Empty)))
            {
                var embedding = await _customAiService.GetEmbeddingAsync(sanitizedText);
                embeddings.Add(new Embedding
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    Text = sanitizedText,
                    Vector = new Vector(embedding)
                });
            }

            await _embeddingRepository.AddRangeEmbeddingsAsync(embeddings);

            documentIsReady = true;
        }
        finally
        {
            await _documentRepository.UpdateDocumentStatusAsync(userId, document.Name, documentIsReady);
        }

        document.IsReadyToUse = documentIsReady;

        var documentWithOwner = _mapper.Map<DocumentResponseWithOwner>(document);

        var user = await _userRepository.GetUserByIdAsync(documentWithOwner.UserId)
                   ?? throw new Exception("User was not found");

        documentWithOwner.Email = user.Email;
        documentWithOwner.FirstName = user.FirstName;
        documentWithOwner.LastName = user.LastName;
        documentWithOwner.AvatarLink = user.AvatarLink;

        return documentWithOwner;
    }

    /// <inheritdoc cref="IDocumentService.ReadPdfAsync" />
    public async Task<string> ReadPdfAsync(Guid userId, string fileName)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(GetFileUrl(userId, fileName));

        using var document = PdfDocument.Open(await response.Content.ReadAsByteArrayAsync());

        var stringBuilder = new StringBuilder();

        foreach (var page in document.GetPages()) stringBuilder.AppendLine(page.Text);

        return stringBuilder.ToString();
    }

    /// <inheritdoc cref="IDocumentService.GetUploadFileUrlAsync" />
    public async Task<string> GetUploadFileUrlAsync(Guid userId, string fileName)
    {
        if (await _documentRepository.GetDocumentByNameAsync(userId, fileName) is not null)
            throw new Exception("File already exist");

        return ReturnUrlWithPermission(userId, fileName, SAS_TTL, BlobSasPermissions.Write);
    }

    /// <inheritdoc cref="IDocumentService.GetFileUrl" />
    public string GetFileUrl(Guid userId, string fileName)
    {
        return ReturnUrlWithPermission(userId, fileName, SAS_TTL, BlobSasPermissions.Read);
    }

    /// <inheritdoc cref="IDocumentService.GetTheClosesContextAsync" />
    public async Task<string> GetTheClosesContextAsync(Guid userId, string searchRequest, string fileName)
    {
        var embedding = await _customAiService.GetEmbeddingAsync(searchRequest);
        var searchResult = await _embeddingRepository.GetTheClosestEmbeddingAsync(userId, fileName, embedding);

        return searchResult?.Text ?? string.Empty;
    }

    /// <inheritdoc cref="IDocumentService.FindUserByEmailAsync" />
    public async Task<List<UserDto>> FindUserByEmailAsync(string emailPrefix)
    {
        return _mapper.Map<List<UserDto>>(await _userRepository.FindUserByEmailAsync(emailPrefix));
    }

    /// <inheritdoc cref="IDocumentService.GivePermissionForUsingDocumentAsync" />
    public async Task GivePermissionForUsingDocumentAsync(Guid ownerId, PermissionDto permissionDto)
    {
        if (ownerId == permissionDto.ReceiverId)
            throw new Exception("You cannot give permission to yourself");

        var document = await _documentRepository.GetDocumentByNameAsync(ownerId, permissionDto.DocumentName)
                       ?? throw new Exception("Document was not found");

        await _documentSharingRepository.AddDocumentSharingAsync(new DocumentSharing
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            UserId = permissionDto.ReceiverId
        });
    }

    public async Task RemovePermissionForUsingDocumentAsync(Guid ownerId, PermissionDto permissionDto)
    {
        if (ownerId == permissionDto.ReceiverId)
            throw new Exception("You cannot remove permission to yourself");

        var document = await _documentRepository.GetDocumentByNameAsync(ownerId, permissionDto.DocumentName)
                       ?? throw new Exception("Document was not found");

        await _documentSharingRepository.DeleteDocumentSharingAsync(permissionDto.ReceiverId, document.Id);
    }

    public async Task<PaginationResponseDto<UserDto>> GetAllUsersWithPermissionAsync(Guid ownerId, string documentName,
        PaginationRequestDto paginationRequest)
    {
        var document = await _documentRepository.GetDocumentByNameAsync(ownerId, documentName)
                       ?? throw new Exception("Document was not found");

        var users = await _documentSharingRepository.GetAllSharedUsersAsync(document.Id);

        return _mapper.Map<PaginationResponseDto<UserDto>>(
            await _userRepository.GetUsersByIdsWithPaginationAsync(users, paginationRequest));
    }

    private string ReturnUrlWithPermission(Guid userId, string fileName, int minutesForExpire,
        BlobSasPermissions permission)
    {
        var blobClient = GetBlobContainerClient(userId, fileName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(minutesForExpire)
        };
        sasBuilder.SetPermissions(permission);

        var conBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = EnvironmentVariables.BlobStorageConnectionString
        };

        var sasToken = sasBuilder
            .ToSasQueryParameters(new StorageSharedKeyCredential(
                conBuilder["AccountName"] as string,
                conBuilder["AccountKey"] as string))
            .ToString();

        return $"{blobClient.Uri}?{sasToken}";
    }

    private List<string> SplitText(string text)
    {
        var textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 4000, chunkOverlap: 50);
        return textSplitter.SplitText(text);
    }

    private BlobClient GetBlobContainerClient(Guid userId, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("private-rag-documents");
        return containerClient.GetBlobClient($"{fileName}-{userId}.pdf");
    }
}