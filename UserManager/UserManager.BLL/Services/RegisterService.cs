using AutoMapper;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.Common.Helpers;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services;

public class RegisterService : IRegisterService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IPaymentRepository _paymentRepository;

    public RegisterService(IMapper mapper, IUserRepository userRepository, IPaymentRepository paymentRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task RegisterUserAsync(UserRegisterRequestDto userRegister)
    {
        var user = _mapper.Map<User>(userRegister);
        user.Password = CryptoHelper.GetHash(userRegister.Password, user.Salt);

        if (await _userRepository.CheckEmailIsExistAsync(user.Email))
            throw new UserAlreadyExistException(user.Email);

        await _userRepository.AddUserAsync(user);

        await _paymentRepository.CreateWalletForNewUserAsync(user.Id);
    }
}