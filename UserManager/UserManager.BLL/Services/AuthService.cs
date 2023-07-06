using AutoMapper;
using UserManager.BLL.Dtos.LoginDtos;
using UserManager.BLL.Dtos.RefreshTokenDtos;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Helpers;
using UserManager.BLL.Interfaces;
using UserManager.Common.Helpers;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services;

public class AuthService : IAuthService
{
	private readonly IMapper _mapper;
	private readonly IUserRepository _userRepository;

	public AuthService(IMapper mapper, IUserRepository userRepository)
	{
		_mapper = mapper;
		_userRepository = userRepository;
	}

	public async Task RegisterUserAsync(UserRegisterRequestDto userRegister)
	{
		userRegister.Email = userRegister.Email.Trim().ToLower();
		
		var user = _mapper.Map<User>(userRegister);
		user.Salt = CryptoHelper.GenerateSalt();
		user.Password = CryptoHelper.GetHash(userRegister.Password, user.Salt);

		if (await _userRepository.CheckEmailIsExistAsync(user.Email))
			throw new UserAlreadyExistException(user.Email);

		await _userRepository.AddUserAsync(user);
	}
	
	public async Task<LoginResponseDto> LoginUser(LoginRequestDto loginUser)
	{
		loginUser.Email = loginUser.Email.Trim().ToLower();
		
		var user = await _userRepository.GetUserByEmailAsync(loginUser.Email) ?? throw new UserNotFoundException(loginUser.Email);

		if (!CryptoHelper.ConfirmPassword(loginUser.Password, user.Salt, user.Password))
			throw new FailedLoginException();

		var loginedUser = _mapper.Map<LoginResponseDto>(user);
		
		loginedUser.Jwt = JwtHelper.GenerateJwt(user);
		
		return loginedUser;
	}
	
	public async Task RefreshToken(RefreshTokenRequestDto refreshTokenRequest)
	{
		
	}
}
