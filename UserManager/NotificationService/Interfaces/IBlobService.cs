using System.Threading.Tasks;

namespace NotificationService.Interfaces
{
    public interface IBlobService
    {
        public Task<string> GetTemplateAsync(string containerName, string path);
    }
}
