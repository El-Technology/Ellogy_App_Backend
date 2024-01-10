using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace AICommunicationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RAGController : Controller
    {
        private readonly DocumentService _documentService;
        private readonly ICommunicationService _communicationService;
        public RAGController(DocumentService documentService, ICommunicationService communicationService)
        {
            _documentService = documentService;
            _communicationService = communicationService;
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

        [HttpGet]
        [Route("readText")]
        public async Task<IActionResult> ReadText([FromQuery] string fileName)
        {
            await _documentService.ReadPdf(fileName);
            return Ok();
        }

        [HttpPost]
        [Route("embedFile")]
        public async Task<IActionResult> EmbedFile([FromBody] string fileName)
        {
            var documentText = await _documentService.ReadPdf(fileName);

            return Ok(await _communicationService.GetEmbeddingAsync(text));
        }
    }
}
