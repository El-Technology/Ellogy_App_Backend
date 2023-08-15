using System.Threading.Tasks;

namespace NotificationService.Interfaces
{
    public interface IBlobService
    {
        Task<byte[]> GetImageFromBlobAsync(string blobUrl, string imagesContainer);
        Task<string> GetTemplateAsync(string containerName, string path);
    }
}
