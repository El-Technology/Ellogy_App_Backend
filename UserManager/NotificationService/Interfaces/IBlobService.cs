using System;
using System.Threading.Tasks;

namespace NotificationService.Interfaces
{
    public interface IBlobService
    {
        Task<BinaryData> GetImageFromBlobAsync(string fileName, string imageContainer);
        Task<string> GetTemplateAsync(string containerName, string templatePath);
    }
}
