using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Dtos.PasswordDtos;
using UserManager.BLL.Interfaces;

namespace UserManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class PasswordController : Controller
{
    private readonly IPasswordService _passwordService;

    public PasswordController(IPasswordService passwordService)
    {
        _passwordService = passwordService;
    }

    [HttpPost]
    [Route("forgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPassword)
    {
        await _passwordService.ForgotPasswordAsync(forgotPassword);
        return Ok();
    }

    [HttpPost]
    [Route("resetPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordDto resetPassword)
    {
        await _passwordService.ResetPasswordAsync(resetPassword);
        return Ok();
    }
}
