using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AICommunicationService.Controllers
{
    /// <summary>
    /// API controller for managing prompts.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PromptController : ControllerBase
    {
        private readonly IPromptService _promptService;
        public PromptController(IPromptService promptService)
        {
            _promptService = promptService;
        }

        /// <summary>
        /// Retrieves all prompts from the service.
        /// </summary>
        /// <returns>Returns a response containing all prompts available in the system.</returns>
        [HttpGet]
        [Route("getAllPrompts")]
        public async Task<IActionResult> GetAllPrompts()
        {
            var response = await _promptService.GetAllPromptsAsync();
            return Ok(response);
        }

        /// <summary>
        /// Creates a new prompt using the provided data.
        /// </summary>
        /// <param name="aiPrompt">The data required to create the prompt.</param>
        /// <returns>Returns a response containing the newly created prompt's details.</returns>
        [HttpPost]
        [Route("createPrompt")]
        public async Task<IActionResult> CreatePrompt([FromBody] CreatePromptDto aiPrompt)
        {
            var response = await _promptService.AddPromptAsync(aiPrompt);
            return Ok(response);
        }

        /// <summary>
        /// Updates an existing prompt with the provided data.
        /// </summary>
        /// <param name="promptName">The name of the prompt to be updated.</param>
        /// <param name="aiPrompt">The updated data for the prompt.</param>
        /// <returns>Returns a response indicating the success of the update operation.</returns>
        [HttpPut]
        [Route("updatePrompt")]
        public async Task<IActionResult> UpdatePrompt(string promptName, [FromBody] UpdatePrompt aiPrompt)
        {
            var response = await _promptService.UpdatePromptAsync(aiPrompt, promptName);
            return Ok(response);
        }

        /// <summary>
        /// Deletes a prompt with the provided data.
        /// </summary>
        /// <param name="promptName">The name of the prompt to be deleted.</param>
        /// <returns>Returns a response indicating the success of the delete operation.</returns>
        [HttpDelete]
        [Route("deletePrompt")]
        public async Task<IActionResult> DeletePrompt(string promptName)
        {
            var response = await _promptService.DeletePromptAsync(promptName);
            return Ok(response);
        }
    }
}
