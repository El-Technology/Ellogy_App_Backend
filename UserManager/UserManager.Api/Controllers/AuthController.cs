using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserManager.BLL.Dtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;

namespace UserManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class AuthController : Controller
{
    private readonly IRegisterService _registerService;
    private readonly ILoginService _loginService;

    public AuthController(IRegisterService registerService, ILoginService loginService)
    {
        _registerService = registerService;
        _loginService = loginService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto userRegister)
    {
        try
        {
            var user = await _registerService.RegisterUser(userRegister);
            return Ok(user);
        }
        catch (UserAlreadyExistException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet]
    [Route("login")]
    public async Task<IActionResult> Login([Required] string email, [Required] string password)
    {
        try
        {
            var jwtToken = await _loginService.LoginUser(email, password);
            return Ok(jwtToken);
        }
        catch (UserNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (FailedLoginException exception)
        {
            return Unauthorized(exception.Message);
        }
    }
}
