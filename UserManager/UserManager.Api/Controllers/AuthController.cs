using Microsoft.AspNetCore.Mvc;
using UserManager.BLL.Dtos;
using UserManager.BLL.Dtos.LoginDtos;
using UserManager.BLL.Interfaces;

namespace UserManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/")]
public class AuthController : Controller
{
    private const string CookieName = "Token";
    private const int CookieExpireInMinutes = 60;

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
        await _registerService.RegisterUserAsync(userRegister);
        return Ok();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginUser)
    {
        var user = await _loginService.LoginUser(loginUser);

        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddMinutes(CookieExpireInMinutes),
            HttpOnly = true,
            Secure = true,
            Path = "/"
        };

        Response.Cookies.Append(CookieName, user.Jwt, cookieOptions);
        return Ok(user);
    }
}
