using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Interfaces.HttpInterfaces;
using AICommunicationService.Common;
using AICommunicationService.Common.Dtos;
using AICommunicationService.DAL.Interfaces;
using AICommunicationService.DAL.Models;
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
    private readonly IUserExternalHttpService _userExternalHttpService;

    public DocumentService(BlobServiceClient blobServiceClient,
        IAzureOpenAiRequestService customAiService,
        IEmbeddingRepository embeddingRepository,
        IDocumentRepository documentRepository,
        IMapper mapper,
        IDocumentSharingRepository documentSharingRepository,
        IUserExternalHttpService userExternalHttpService)
    {
        _documentSharingRepository = documentSharingRepository;
        _mapper = mapper;
        _documentRepository = documentRepository;
        _embeddingRepository = embeddingRepository;
        _customAiService = customAiService;
        _blobServiceClient = blobServiceClient;
        _userExternalHttpService = userExternalHttpService;
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
            await httpClient.DeleteAsync(await ReturnUrlWithPermissionAsync(userId, fileName, 5, BlobSasPermissions.Delete));

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

        var users = await _userExternalHttpService.GetUsersByIdsAsync(documentResponse.Data.Select(a => a.UserId).ToList());

        foreach (var document in documentResponse.Data)
        {
            var user = users.FirstOrDefault(a => a.Id == document.UserId);

            ArgumentNullException.ThrowIfNull(user, "User was not found");

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

        var user = await _userExternalHttpService.GetUserByIdAsync(documentWithOwner.UserId)
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
        var response = await httpClient.GetAsync(await GetFileUrlAsync(userId, fileName));

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

        return await ReturnUrlWithPermissionAsync(userId, fileName, SAS_TTL, BlobSasPermissions.Write);
    }

    /// <inheritdoc cref="IDocumentService.GetFileUrlAsync" />
    public async Task<string> GetFileUrlAsync(Guid userId, string fileName)
    {
        return await ReturnUrlWithPermissionAsync(userId, fileName, SAS_TTL, BlobSasPermissions.Read);
    }

    /// <inheritdoc cref="IDocumentService.GetTheClosesContextAsync" />
    public async Task<string> GetTheClosesContextAsync(Guid userId, string searchRequest, string fileName)
    {
        var embedding = await _customAiService.GetEmbeddingAsync(searchRequest);
        var embeddingVector = new Vector(embedding);

        var searchResult = await _embeddingRepository.GetTheClosestEmbeddingAsync(userId, fileName, embeddingVector);

        var stringBuilder = new StringBuilder();
        foreach (var result in searchResult) stringBuilder.AppendLine(result.Text);

        return stringBuilder.ToString();
    }

    /// <inheritdoc cref="IDocumentService.FindUserByEmailAsync" />
    public async Task<List<UserDto>> FindUserByEmailAsync(string emailPrefix)
    {
        return await _userExternalHttpService.FindUserByEmailAsync(emailPrefix);
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

        return await _userExternalHttpService.GetUsersByIdsWithPaginationAsync(users, paginationRequest);
    }

    private async Task<string> ReturnUrlWithPermissionAsync(Guid userId, string fileName, int minutesForExpire,
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
            ConnectionString = await EnvironmentVariables.GetBlobStorageConnectionStringAsync
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
        var textSplitter = new RecursiveCharacterTextSplitter(chunkSize: 1500, chunkOverlap: 100);
        return textSplitter.SplitText(text);
    }

    private BlobClient GetBlobContainerClient(Guid userId, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("private-rag-documents");
        return containerClient.GetBlobClient($"{fileName}-{userId}.pdf");
    }
}