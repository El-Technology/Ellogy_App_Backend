using AutoMapper;
using UserManager.BLL.Dtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Helpers;
using UserManager.BLL.Interfaces;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services;

public class RegisterService : IRegisterService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public RegisterService(IMapper mapper, IUserRepository userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<UserRegisterResponseDto> RegisterUser(UserRegisterRequestDto userRegister)
    {
        var user = _mapper.Map<User>(userRegister);
        user.Password = CryptoHelper.GetHashPassword(userRegister.Password, user.Salt);

        if (await _userRepository.CheckEmail(user.Email))
            throw new UserAlreadyExistException(user.Email);

        await _userRepository.AddUserAsync(user);
        var responseUser = _mapper.Map<UserRegisterResponseDto>(user);

        return responseUser;
    }
}