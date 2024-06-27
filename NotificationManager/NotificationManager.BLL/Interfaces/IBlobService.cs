namespace NotificationManager.BLL.Interfaces;

public interface IBlobService
{
    Task<BinaryData> GetImageFromBlobAsync(string fileName, string imageContainer);
}