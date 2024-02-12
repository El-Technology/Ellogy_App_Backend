using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common;
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
    /// <returns></returns>
    [HttpGet]
    [Route("getDocumentUrl")]
    public IActionResult GetUrlOfPdfDocuments([FromQuery] string fileName)
    {
        return Ok(_documentService.GetFileUrl(GetUserIdFromToken(), fileName));
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
    /// <returns></returns>
    [HttpGet]
    [Route("getUserDocuments")]
    public async Task<IActionResult> GetUserDocuments()
    {
        return Ok(await _documentService.GetAllUserDocumentsAsync(GetUserIdFromToken()));
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
}