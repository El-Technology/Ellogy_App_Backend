using System;
using System.Threading.Tasks;

namespace NotificationService.Interfaces
{
    /// <summary>
    /// Interface for managing operations related to Azure Blob Storage.
    /// </summary>
    public interface IBlobService
    {
        /// <summary>
        /// Retrieves an image from the specified blob container asynchronously.
        /// </summary>
        /// <param name="fileName">The name of the image file to retrieve.</param>
        /// <param name="imageContainer">The name of the blob container containing the image.</param>
        /// <returns>A task representing the asynchronous operation. The binary data of the retrieved image.</returns>
        Task<BinaryData> GetImageFromBlobAsync(string fileName, string imageContainer);

        /// <summary>
        /// Retrieves the content of a template file from the specified blob container asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the blob container containing the template file.</param>
        /// <param name="templatePath">The path to the template file within the container.</param>
        /// <returns>A task representing the asynchronous operation. The content of the retrieved template file as a string.</returns>
        Task<string> GetTemplateAsync(string containerName, string templatePath);
    }
}
