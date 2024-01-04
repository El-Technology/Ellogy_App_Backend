using AutoMapper;
using Microsoft.Azure.Amqp.Framing;
using System.Web;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.Common.Helpers;
using UserManager.Common.Models.NotificationModels;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services;

public class RegisterService : IRegisterService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly INotificationQueueService _notificationQueueService;
    private readonly NotificationModel _notificationModel = new()
    {
        Type = NotificationTypeEnum.VerifyEmail,
        Way = NotificationWayEnum.Email
    };
    private const string VerifyEmailTemplate = "{0}?email={1}&token={2}";
    private const string VerifyEmailAddressLink = "{{{verifyEmailAddressLink}}}";
    private const string FirstName = "{{{firstName}}}";

    public RegisterService(IMapper mapper, IUserRepository userRepository, IPaymentRepository paymentRepository, INotificationQueueService notificationQueueService)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _paymentRepository = paymentRepository;
        _notificationQueueService = notificationQueueService;
    }

    public async Task RegisterUserAsync(UserRegisterRequestDto userRegister)
    {
        var user = _mapper.Map<User>(userRegister);
        user.Password = CryptoHelper.GetHash(userRegister.Password, user.Salt);

        if (await _userRepository.CheckEmailIsExistAsync(user.Email))
            throw new UserAlreadyExistException(user.Email);

        var token = CryptoHelper.GenerateToken();
        user.VerifyToken = token;
        user.IsAccountActivated = false;

        await _userRepository.AddUserAsync(user);
    }

    public async Task SendVerificationEmailAsync(SendVerificationEmailDto sendVerificationEmailDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(sendVerificationEmailDto.UserEmail)
            ?? throw new UserNotFoundException();

        var hashToken = CryptoHelper.GetHash(user.VerifyToken);

        var verifyEmailUrl = string.Format(VerifyEmailTemplate, sendVerificationEmailDto.RedirectLink,
            HttpUtility.UrlEncode(sendVerificationEmailDto.UserEmail), HttpUtility.UrlEncode(hashToken));

        _notificationModel.Consumer = user.Email;
        _notificationModel.MetaData = new() { { FirstName, user.FirstName }, { VerifyEmailAddressLink, verifyEmailUrl } };

        await _notificationQueueService.SendNotificationAsync(_notificationModel);
    }

    public async Task ActivateUserAccountAsync(ActivateUserAccountDto activateUser)
    {
        activateUser.Token = HttpUtility.UrlDecode(activateUser.Token);

        var user = await _userRepository.GetUserByEmailAsync(activateUser.UserEmail)
            ?? throw new UserNotFoundException();

        if (!(CryptoHelper.GetHash(user.VerifyToken) == activateUser.Token))
            throw new Exception("Wrong activate token");

        user.IsAccountActivated = true;

        await _userRepository.UpdateUserAsync(user);
        await _paymentRepository.CreateWalletForNewUserAsync(user.Id);
    }
}