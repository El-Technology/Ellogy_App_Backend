using AutoMapper;
using UserManager.BLL.Dtos.LoginDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Helpers;
using UserManager.BLL.Interfaces;
using UserManager.Common.Helpers;
using UserManager.DAL.Interfaces;

namespace UserManager.BLL.Services;

public class LoginService : ILoginService
{
    private readonly IMapper _mapper;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserRepository _userRepository;

    public LoginService(IUserRepository userRepository, IMapper mapper, IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <inheritdoc cref="ILoginService.LoginUserAsync" />
    public async Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginUser)
    {
        if (!EmailHelper.IsValidEmail(loginUser.Email))
            throw new InvalidEmailException();

        var user = await _userRepository.GetUserByEmailAsync(loginUser.Email) ??
                   throw new UserNotFoundException(loginUser.Email);

        if (!user.IsAccountActivated)
            throw new EmailVerificationException();

        if (!CryptoHelper.ConfirmPassword(loginUser.Password, user.Salt, user.Password))
            throw new FailedLoginException();

        var loggedInUser = _mapper.Map<LoginResponseDto>(user);
        loggedInUser.Jwt = await JwtHelper.GenerateJwtAsync(user);
        loggedInUser.RefreshToken = await _refreshTokenService.GetRefreshTokenAsync(user.Id);

        return loggedInUser;
    }
}