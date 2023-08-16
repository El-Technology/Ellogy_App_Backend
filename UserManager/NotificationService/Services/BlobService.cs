using Azure.Storage.Blobs;
using NotificationService.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using UserManager.Common.Constants;

namespace NotificationService.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        private async Task<BinaryData> DownloadFromBlobAsync(string containerName, string pathToFile)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainer.GetBlobClient(pathToFile);
            var downloadedContent = await blobClient.DownloadContentAsync();
            var contentForReturn = downloadedContent.Value.Content;

            if (containerName.Equals(BlobContainerConstants.ImagesContainer))
                await blobClient.DeleteIfExistsAsync();

            return contentForReturn;
        }

        public async Task<string> GetTemplateAsync(string containerName, string path)
        {
            var template = (await DownloadFromBlobAsync(containerName, path)).ToString();
            return template;
        }

        public async Task<BinaryData> GetImageFromBlobAsync(string fileName, string imageContainer)
        {
            return await DownloadFromBlobAsync(imageContainer, fileName);
        }
    }
}
