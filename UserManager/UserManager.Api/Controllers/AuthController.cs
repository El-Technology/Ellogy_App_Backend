using Microsoft.AspNetCore.Mvc;
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

    /// <summary>
    /// Registers a new user. Returns HTTP 404 if user already exist. 
    /// </summary>
    /// <param name="userRegister">The user registration data.</param>
    /// <returns><see cref="StatusCodes.Status200OK"/> or
    /// <br/><see cref="StatusCodes.Status404NotFound"/> or <see cref="StatusCodes.Status500InternalServerError"/></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto userRegister)
    {
        await _registerService.RegisterUserAsync(userRegister);
        return Ok();
    }

    /// <summary>
    /// Logs in a user with the provided credentials.
    /// </summary>
    /// <param name="loginUser">The login credentials of the user.</param>
    /// <returns>An object that represent User <see cref="LoginResponseDto"/> with JWT token for future authorization</returns>
    /// <remarks>
    /// This method attempts to log in a user by verifying the provided credentials against the user database. If the login is successful,
    /// a JSON Web Token (JWT) is generated for the user and stored in an HTTP-only secure cookie. The user's JWT can be used for subsequent
    /// authenticated requests.
    /// </remarks>
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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
