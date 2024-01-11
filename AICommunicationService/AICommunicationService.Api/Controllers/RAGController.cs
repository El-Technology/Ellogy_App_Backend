using AICommunicationService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AICommunicationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RAGController : Controller
    {
        private readonly IDocumentService _documentService;
        public RAGController(IDocumentService documentService, ICommunicationService communicationService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        [Route("uploadDocumentUrl")]
        public IActionResult GetUrlForPutPDFDocuments([FromQuery] string fileName)
        {
            return Ok(_documentService.GetUploadFileUrl(fileName));
        }

        [HttpGet]
        [Route("getDocumentUrl")]
        public IActionResult GetUrlOfPDFDocuments([FromQuery] string fileName)
        {
            return Ok(_documentService.GetFileUrl(fileName));
        }

        [HttpGet]
        [Route("deleteDocumentUrl")]
        public IActionResult GetUrlForDeletePDFDocuments([FromQuery] string fileName)
        {
            return Ok(_documentService.GetDeleteFileUrl(fileName));
        }

        [HttpPost]
        [Route("embedFile")]
        public async Task<IActionResult> EmbedFile([FromBody] string fileName)
        {
            await _documentService.InsertDocumentContextInVectorDbAsync(fileName, Guid.NewGuid()); // add user id 
            return Ok();
        }
    }
}
