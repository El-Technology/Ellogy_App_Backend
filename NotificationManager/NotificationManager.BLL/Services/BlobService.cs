using Azure.Storage.Blobs;
using NotificationManager.BLL.Interfaces;

namespace NotificationManager.BLL.Services;

/// <summary>
/// Service for interacting with blobs
/// </summary>
public class BlobService : IBlobService
{
    private BlobServiceClient _blobServiceClient;

    /// <summary>
    /// Constructor for the blob service
    /// </summary>
    /// <param name="blobServiceClient"></param>
    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    /// <summary>
    /// Downloads a file from a blob container
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="pathToFile"></param>
    /// <returns></returns>
    private async Task<BinaryData> DownloadFromBlobAsync(string containerName, string pathToFile)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainer.GetBlobClient(pathToFile);
        var downloadedContent = await blobClient.DownloadContentAsync();
        var contentForReturn = downloadedContent.Value.Content;

        return contentForReturn;
    }

    /// <inheritdoc cref="IBlobService.GetImageFromBlobAsync(string, string)"/>
    public async Task<BinaryData> GetImageFromBlobAsync(string fileName, string imageContainer)
    {
        return await DownloadFromBlobAsync(imageContainer, fileName);
    }
}