using AutoMapper;
using UserManager.BLL.Dtos.LoginDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Helpers;
using UserManager.BLL.Interfaces;
using UserManager.Common.Helpers;
using UserManager.DAL.Enums;
using UserManager.DAL.Interfaces;

namespace UserManager.BLL.Services;

public class LoginService : ILoginService
{
    private readonly IMapper _mapper;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserRepository _userRepository;

    public LoginService(IUserRepository userRepository, IMapper mapper, IRefreshTokenService refreshTokenService,
        IPaymentRepository paymentRepository)
    {
        _refreshTokenService = refreshTokenService;
        _userRepository = userRepository;
        _mapper = mapper;
        _paymentRepository = paymentRepository;
    }

    /// <inheritdoc cref="ILoginService.LoginUserAsync" />
    public async Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginUser)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginUser.Email) ??
                   throw new UserNotFoundException(loginUser.Email);

        if (!user.IsAccountActivated)
            throw new EmailVerificationException();

        if (user.Role != RoleEnum.Admin)
            throw new Exception("You are not allowed to login");

        if (!CryptoHelper.ConfirmPassword(loginUser.Password, user.Salt, user.Password))
            throw new FailedLoginException();

        if (!await _paymentRepository.CheckIfUserHaveWalletAsync(user.Id))
            await _paymentRepository.CreateWalletForNewUserAsync(user.Id);

        var loginedUser = _mapper.Map<LoginResponseDto>(user);
        loginedUser.Jwt = JwtHelper.GenerateJwt(user);
        loginedUser.RefreshToken = await _refreshTokenService.GetRefreshTokenAsync(user.Id);

        return loginedUser;
    }
}