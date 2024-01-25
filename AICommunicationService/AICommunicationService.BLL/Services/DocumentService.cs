using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common;
using AICommunicationService.DAL.Models;
using AICommunicationService.RAG.Interfaces;
using AICommunicationService.RAG.Models;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using LangChain.TextSplitters;
using System.Data.Common;
using System.Text;
using UglyToad.PdfPig;

namespace AICommunicationService.BLL.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IAzureOpenAiRequestService _customAiService;
        private readonly IEmbeddingRepository _embeddingRepository;
        private readonly IDocumentRepository _documentRepository;
        public DocumentService(BlobServiceClient blobServiceClient,
                               IAzureOpenAiRequestService customAiService,
                               IEmbeddingRepository embeddingRepository,
                               IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
            _embeddingRepository = embeddingRepository;
            _customAiService = customAiService;
            _blobServiceClient = blobServiceClient;
        }

        private string ReturnUrlWithPermission(string fileName, int minutesForExpire, BlobSasPermissions permission)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("private-rag-documents");
            var blobClient = containerClient.GetBlobClient($"{fileName}.pdf");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
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

        public string GetUploadFileUrl(string fileName)
        {
            return ReturnUrlWithPermission(fileName, 5, BlobSasPermissions.Write);
        }

        public string GetFileUrl(string fileName)
        {
            return ReturnUrlWithPermission(fileName, 5, BlobSasPermissions.Read);
        }

        public async Task DeleteFileAsync(Guid userId, string fileName)
        {
            var document = await _documentRepository.GetDocumentByNameAsync(fileName)
                ?? throw new ArgumentNullException(nameof(fileName));

            if (userId != document.UserId)
                throw new Exception("You don`t have access to delete this file");

            var httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync(ReturnUrlWithPermission(fileName, 5, BlobSasPermissions.Delete));

            if (!response.IsSuccessStatusCode)
                throw new Exception("Delete file error, try again");

            await _embeddingRepository.DeleteEmbeddingsAsync(fileName);
            await _documentRepository.DeleteDocumentAsync(fileName);
        }

        public async Task<string> ReadPdf(string fileName)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(GetFileUrl(fileName));

            using var document = PdfDocument.Open(await response.Content.ReadAsByteArrayAsync());

            var stringBuilder = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                stringBuilder.AppendLine(page.Text);
            }

            return stringBuilder.ToString();
        }

        public async Task InsertDocumentContextInVectorDbAsync(string fileName, Guid userId)
        {
            var document = await _documentRepository.GetDocumentByNameAsync(fileName)
                ?? throw new Exception("Document was not found");

            var documentContext = await ReadPdf(fileName);

            var splitText = SplitText(documentContext);

            var embeddings = new List<Embedding>();

            foreach (var text in splitText)
            {
                var embedding = await _customAiService.GetEmbeddingAsync(text);
                embeddings.Add(new Embedding
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    Text = text,
                    Vector = new Pgvector.Vector(embedding)
                });
            }

            await _embeddingRepository.AddRangeEmbeddingsAsync(embeddings);
        }

        public async Task CheckIfDocumentWasUploadedAsync(Guid userId, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("private-rag-documents");
            var blobClient = containerClient.GetBlobClient($"{fileName}.pdf");

            if (!await blobClient.ExistsAsync())
                throw new Exception($"File with name:{fileName} cannot be found, try again later");

            var document = new Document
            {
                Id = Guid.NewGuid(),
                Name = fileName,
                UserId = userId
            };

            await _documentRepository.AddDocumentAsync(document);
        }

        public async Task<string> GetTheClosesContextAsync(string searchRequest, string fileName)
        {
            var embedding = await _customAiService.GetEmbeddingAsync(searchRequest);
            var searchResult = await _embeddingRepository.GetTheClosestEmbeddingAsync(fileName, embedding);
            return searchResult!.Text;
        }

        public async Task<List<string>> GetAllUserDocumentsAsync(Guid userId)
        {
            return await _documentRepository.GetAllUserDocumentsAsync(userId);
        }
    }
}
