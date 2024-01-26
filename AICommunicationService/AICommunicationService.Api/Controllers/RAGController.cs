using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AICommunicationService.Controllers
{

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
        /// This method retrieves the user id from the JWT token
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Guid GetUserIdFromToken()
        {
            var status = Guid.TryParse(User.FindFirst(JwtOptions.UserIdClaimName)?.Value, out Guid userId);
            if (!status)
                throw new Exception("Taking user id error, try again later");

            return userId;
        }

        [HttpGet]
        [Route("uploadDocumentUrl")]
        public async Task<IActionResult> GetUrlForPutPDFDocuments([FromQuery] string fileName)
        {
            return Ok(await _documentService.GetUploadFileUrlAsync(fileName));
        }

        [HttpGet]
        [Route("getDocumentUrl")]
        public IActionResult GetUrlOfPDFDocuments([FromQuery] string fileName)
        {
            return Ok(_documentService.GetFileUrl(fileName));
        }

        [HttpDelete]
        [Route("deleteDocument")]
        public async Task<IActionResult> DeletePDFDocument([FromQuery] string fileName)
        {
            await _documentService.DeleteFileAsync(GetUserIdFromToken(), fileName);
            return Ok();
        }

        [HttpGet]
        [Route("getUserDocuments")]
        public async Task<IActionResult> GetUserDocuments()
        {
            return Ok(await _documentService.GetAllUserDocumentsAsync(GetUserIdFromToken()));
        }

        [HttpGet]
        [Route("verifyDocumentUpload")]
        public async Task<IActionResult> VerifyDocumentUpload([FromQuery] string fileName)
        {
            await _documentService.CheckIfDocumentWasUploadedAsync(GetUserIdFromToken(), fileName);
            return Ok();
        }

        [HttpGet]
        [Route("embedDocument")]
        public async Task<IActionResult> EmbedDocument([FromQuery] string fileName)
        {
            return Ok(await _documentService.InsertDocumentContextInVectorDbAsync(fileName, GetUserIdFromToken()));
        }
    }
}
