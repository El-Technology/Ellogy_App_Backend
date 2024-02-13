using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common;
using AICommunicationService.Common.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AICommunicationService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RAGController : Controller
{
    private readonly IDocumentService _documentService;

    public RAGController(IDocumentService documentService, ICommunicationService communicationService)
    {
        _documentService = documentService;
    }

    /// <summary>
    ///     This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken()
    {
        var status = Guid.TryParse(User.FindFirst(JwtOptions.UserIdClaimName)?.Value, out var userId);
        if (!status)
            throw new Exception("Taking user id error, try again later");

        return userId;
    }

    /// <summary>
    ///     This method returns the url for uploading the document
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("uploadDocumentUrl")]
    public async Task<IActionResult> GetUrlForPutPdfDocuments([FromQuery] string fileName)
    {
        return Ok(await _documentService.GetUploadFileUrlAsync(GetUserIdFromToken(), fileName));
    }

    /// <summary>
    ///     This method returns the url for downloading the document
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("getDocumentUrl")]
    public IActionResult GetUrlOfPdfDocuments([FromQuery] string fileName, [FromQuery] Guid? ownerId)
    {
        var userId = ownerId ?? GetUserIdFromToken();
        return Ok(_documentService.GetFileUrl(userId, fileName));
    }

    /// <summary>
    ///     This method deletes the document from the storage
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("deleteDocument")]
    public async Task<IActionResult> DeletePdfDocument([FromQuery] string fileName)
    {
        await _documentService.DeleteFileAsync(GetUserIdFromToken(), fileName);
        return Ok();
    }

    /// <summary>
    ///     This method returns the list of the user documents
    /// </summary>
    /// <param name="paginationRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("getUserDocuments")]
    public async Task<IActionResult> GetUserDocuments([FromBody] PaginationRequestDto paginationRequest)
    {
        return Ok(await _documentService.GetAllUserDocumentsAsync(GetUserIdFromToken(), paginationRequest));
    }


    /// <summary>
    ///     This method verifies if the document was uploaded
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("verifyDocumentUpload")]
    public async Task<IActionResult> VerifyDocumentUpload([FromQuery] string fileName)
    {
        await _documentService.CheckIfDocumentWasUploadedAsync(GetUserIdFromToken(), fileName);
        return Ok();
    }

    /// <summary>
    ///     This method embeds the document in the vector database
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("embedDocument")]
    public async Task<IActionResult> EmbedDocument([FromQuery] string fileName)
    {
        return Ok(await _documentService.InsertDocumentContextInVectorDbAsync(GetUserIdFromToken(), fileName));
    }

    /// <summary>
    ///     This method finds the user by email prefix
    /// </summary>
    /// <param name="emailPrefix"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("findUser")]
    public async Task<IActionResult> FindUser([FromQuery] string emailPrefix)
    {
        return Ok(await _documentService.FindUserByEmailAsync(emailPrefix));
    }

    /// <summary>
    ///     This method gives permission for using the document
    /// </summary>
    /// <param name="permissionDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("givePermission")]
    public async Task<IActionResult> GivePermission([FromBody] PermissionDto permissionDto)
    {
        await _documentService.GivePermissionForUsingDocumentAsync(GetUserIdFromToken(), permissionDto);
        return Ok();
    }

    /// <summary>
    ///     This method removes permission for using the document
    /// </summary>
    /// <param name="permissionDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("removePermission")]
    public async Task<IActionResult> RemovePermission([FromBody] PermissionDto permissionDto)
    {
        await _documentService.RemovePermissionForUsingDocumentAsync(GetUserIdFromToken(), permissionDto);
        return Ok();
    }

    /// <summary>
    ///     This method returns the list of the users with permission for using the document
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="paginationRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("getAllUsersWithPermission")]
    public async Task<IActionResult> GetAllUsersWithPermission([FromQuery] string fileName,
        [FromBody] PaginationRequestDto paginationRequest)
    {
        return Ok(await _documentService.GetAllUsersWithPermissionAsync(GetUserIdFromToken(), fileName,
            paginationRequest));
    }
}