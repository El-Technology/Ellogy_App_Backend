using AICommunicationService.BLL.Dtos;

namespace AICommunicationService.BLL.Interfaces;

/// <summary>
///     This interface provides the methods for the document service
/// </summary>
public interface IDocumentService
{
    /// <summary>
    ///     This method checks if the document was uploaded
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task CheckIfDocumentWasUploadedAsync(Guid userId, string fileName);

    /// <summary>
    ///     This method returns the list of the user documents
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<DocumentResponseDto>> GetAllUserDocumentsAsync(Guid userId);

    /// <summary>
    ///     This method deletes the document from the storage
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task DeleteFileAsync(Guid userId, string fileName);

    /// <summary>
    ///     This method returns the url for downloading the document
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    string GetFileUrl(Guid userId, string fileName);

    /// <summary>
    ///     This method returns the url for uploading the document
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="searchRequest"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<string> GetTheClosesContextAsync(Guid userId, string searchRequest, string fileName);

    /// <summary>
    ///     This method returns the url for uploading the document
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<string> GetUploadFileUrlAsync(Guid userId, string fileName);

    /// <summary>
    ///     This method inserts the document context in the vector db
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<DocumentResponseDto> InsertDocumentContextInVectorDbAsync(Guid userId, string fileName);

    /// <summary>
    ///     This method reads the pdf
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<string> ReadPdf(Guid userId, string fileName);
}