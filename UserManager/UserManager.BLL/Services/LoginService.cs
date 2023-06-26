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
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public LoginService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<LoginResponseDto> LoginUser(LoginRequestDto loginUser)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginUser.Email) ?? throw new UserNotFoundException(loginUser.Email);

        if (!CryptoHelper.ConfirmPassword(loginUser.Password, user.Salt, user.Password))
            throw new FailedLoginException();

        var loginedUser = _mapper.Map<LoginResponseDto>(user);
        loginedUser.Jwt = JwtHelper.GenerateJwt(user);

        return loginedUser;
    }
}
