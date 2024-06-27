using Azure.Storage.Blobs;
using NotificationManager.BLL.Interfaces;

namespace NotificationManager.BLL.Services;
public class BlobService : IBlobService
{
    private BlobServiceClient _blobServiceClient;

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

        return contentForReturn;
    }

    public async Task<BinaryData> GetImageFromBlobAsync(string fileName, string imageContainer)
    {
        return await DownloadFromBlobAsync(imageContainer, fileName);
    }
}