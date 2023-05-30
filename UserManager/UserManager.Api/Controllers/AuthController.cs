using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Dtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;

namespace UserManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class AuthController : Controller
{
    private readonly IRegisterService _registerService;

    public AuthController(IRegisterService registerService)
    {
        _registerService = registerService;
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
}
