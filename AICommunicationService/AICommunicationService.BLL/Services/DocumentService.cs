using System.Data.Common;
using System.Text;
using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common;
using AICommunicationService.RAG.Interfaces;
using AICommunicationService.RAG.Models;
using AutoMapper;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using LangChain.TextSplitters;
using Pgvector;
using UglyToad.PdfPig;

namespace AICommunicationService.BLL.Services;

public class DocumentService : IDocumentService
{
    private const int SAS_TTL = 5;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IAzureOpenAiRequestService _customAiService;
    private readonly IDocumentRepository _documentRepository;
    private readonly IEmbeddingRepository _embeddingRepository;
    private readonly IMapper _mapper;

    public DocumentService(BlobServiceClient blobServiceClient,
        IAzureOpenAiRequestService customAiService,
        IEmbeddingRepository embeddingRepository,
        IDocumentRepository documentRepository,
        IMapper mapper)
    {
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
    public async Task<List<DocumentResponseDto>> GetAllUserDocumentsAsync(Guid userId)
    {
        return _mapper.Map<List<DocumentResponseDto>>(await _documentRepository.GetAllUserDocumentsAsync(userId));
    }

    /// <inheritdoc cref="IDocumentService.InsertDocumentContextInVectorDbAsync" />
    public async Task<DocumentResponseDto> InsertDocumentContextInVectorDbAsync(Guid userId, string fileName)
    {
        var documentIsReady = false;

        var document = await _documentRepository.GetDocumentByNameAsync(userId, fileName)
                       ?? throw new Exception("Document was not found");

        if (await _embeddingRepository.CheckIfEmbeddingAlreadyExistAsync(userId, fileName))
            throw new Exception("Embedding already exist");

        try
        {
            var documentContext = await ReadPdf(userId, fileName);

            Console.WriteLine(documentContext);

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
        return _mapper.Map<DocumentResponseDto>(document);
    }

    /// <inheritdoc cref="IDocumentService.ReadPdf" />
    public async Task<string> ReadPdf(Guid userId, string fileName)
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
        return searchResult!.Text;
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