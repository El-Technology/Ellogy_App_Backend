namespace NotificationManager.BLL.Interfaces;

public interface IBlobService
{
    Task<BinaryData> GetImageFromBlobAsync(string fileName, string imageContainer);
    Task<string> GetTemplateAsync(string containerName, string path);
}