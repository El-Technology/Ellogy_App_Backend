using AICommunicationService.Common;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System.Data.Common;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Writer;

namespace AICommunicationService.BLL.Services
{
    public class DocumentService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public DocumentService(BlobServiceClient blobServiceClient)
        {
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

        public string GetUploadFileUrl(string fileName)
        {
            return ReturnUrlWithPermission(fileName, 5, BlobSasPermissions.Write);
        }

        public string GetFileUrl(string fileName)
        {
            return ReturnUrlWithPermission(fileName, 5, BlobSasPermissions.Read);
        }

        public string GetDeleteFileUrl(string fileName)
        {
            return ReturnUrlWithPermission(fileName, 5, BlobSasPermissions.Delete);
        }

        public async Task ReadPdf(string fileName)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(GetFileUrl(fileName));

            using var document = PdfDocument.Open(await response.Content.ReadAsByteArrayAsync());

            foreach (var page in document.GetPages())
            {
                Console.WriteLine(page.Text);
            }
        }
    }
}
