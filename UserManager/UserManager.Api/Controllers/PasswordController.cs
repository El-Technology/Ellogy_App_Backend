using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Dtos.PasswordDtos;
using UserManager.BLL.Interfaces;

namespace UserManager.Api.Controllers;

/// <summary>
///     Controller for handling password-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]/")]
public class PasswordController : Controller
{
    private readonly IPasswordService _passwordService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PasswordController" /> class.
    /// </summary>
    /// <param name="passwordService"></param>
    public PasswordController(IPasswordService passwordService)
    {
        _passwordService = passwordService;
    }

    /// <summary>
    ///     Initiates the Forgot Password process.
    /// </summary>
    /// <param name="forgotPassword">The ForgotPasswordDto containing the user's email address.</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Route("forgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPassword)
    {
        await _passwordService.ForgotPasswordAsync(forgotPassword);
        return Ok();
    }

    /// <summary>
    ///     Resets the user's password.
    /// </summary>
    /// <param name="resetPassword">The ResetPasswordDto containing the forgot password entry id, token, and new password.</param>
    /// <returns>
    ///     <see cref="StatusCodes.Status200OK" /> if password reset is successful or
    ///     <br /><see cref="StatusCodes.Status404NotFound" /> or <see cref="StatusCodes.Status500InternalServerError" />
    /// </returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [HttpPost]
    [Route("resetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
    {
        await _passwordService.ResetPasswordAsync(resetPassword);
        return Ok();
    }
}