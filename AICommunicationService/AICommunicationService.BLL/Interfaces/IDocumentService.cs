using AICommunicationService.BLL.Dtos;
using AICommunicationService.Common.Dtos;

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
    /// <param name="paginationRequest"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<DocumentResponseWithOwner>> GetAllUserDocumentsAsync(Guid userId,
        PaginationRequestDto paginationRequest);

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
    Task<string> GetFileUrlAsync(Guid userId, string fileName);

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
    Task<DocumentResponseWithOwner> InsertDocumentContextInVectorDbAsync(Guid userId, string fileName);

    /// <summary>
    ///     This method reads the pdf
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<string> ReadPdfAsync(Guid userId, string fileName);

    /// <summary>
    ///     This method is used for finding user by email prefix
    /// </summary>
    /// <param name="emailPrefix"></param>
    /// <returns></returns>
    Task<List<UserDto>> FindUserByEmailAsync(string emailPrefix);

    /// <summary>
    ///     This method give another user permission to use the document
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="permissionDto"></param>
    /// <returns></returns>
    Task GivePermissionForUsingDocumentAsync(Guid ownerId, PermissionDto permissionDto);

    /// <summary>
    ///     This method removes permission for using the document
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="permissionDto"></param>
    /// <returns></returns>
    Task RemovePermissionForUsingDocumentAsync(Guid ownerId, PermissionDto permissionDto);

    /// <summary>
    ///     This method returns the list of the users with permission
    /// </summary>
    /// <param name="ownerId"></param>
    /// <param name="documentName"></param>
    /// <param name="paginationRequest"></param>
    /// <returns></returns>
    Task<PaginationResponseDto<UserDto>> GetAllUsersWithPermissionAsync(Guid ownerId, string documentName,
        PaginationRequestDto paginationRequest);
}