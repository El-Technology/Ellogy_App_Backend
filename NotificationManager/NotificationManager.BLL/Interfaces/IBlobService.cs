namespace NotificationManager.BLL.Interfaces;

/// <summary>
/// Interface for the blob service
/// </summary>
public interface IBlobService
{
    /// <summary>
    /// Retrieves images from a blob container
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="imageContainer"></param>
    /// <returns></returns>
    Task<BinaryData> GetImageFromBlobAsync(string fileName, string imageContainer);
}