using Azure.Storage.Blobs;
using NotificationService.Interfaces;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> GetTemplateAsync(string containerName, string path)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainer.GetBlobClient(path);
            var downloadedContent = await blobClient.DownloadContentAsync();
            var template = downloadedContent.Value.Content.ToString();
            return template;
        }
    }
}
