using AutoMapper;
using System.Web;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Exceptions;
using UserManager.BLL.Interfaces;
using UserManager.Common;
using UserManager.Common.Helpers;
using UserManager.Common.Models.NotificationModels;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.BLL.Services;

/// <summary>
///     Service for register operations
/// </summary>
public class RegisterService : IRegisterService
{
    private const string VerifyEmailTemplate = "{0}?email={1}&token={2}";
    private const string VerifyEmailAddressLink = "{{{verifyEmailAddressLink}}}";
    private const string FirstName = "{{{firstName}}}";
    private readonly IMapper _mapper;

    private readonly NotificationModel _notificationModel = new()
    {
        Type = NotificationTypeEnum.VerifyEmail,
        Way = NotificationWayEnum.Email
    };

    private readonly IUserRepository _userRepository;
    private readonly IServiceBusQueue _notificationQueueService;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="userRepository"></param>
    /// <param name="paymentRepository"></param>
    /// <param name="notificationQueueService"></param>
    public RegisterService(IMapper mapper,
        IUserRepository userRepository,
        IServiceBusQueue notificationQueueService)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _notificationQueueService = notificationQueueService;
    }

    /// <inheritdoc cref="IRegisterService.RegisterUserAsync" />
    public async Task RegisterUserAsync(UserRegisterRequestDto userRegister)
    {
        var user = _mapper.Map<User>(userRegister);

        if (await _userRepository.CheckEmailIsExistAsync(user.Email))
            throw new UserAlreadyExistException(user.Email);

        user.Password = CryptoHelper.GetHash(userRegister.Password, user.Salt);
        var token = CryptoHelper.GenerateToken();
        user.VerifyToken = token;
        user.IsAccountActivated = false;

        if (!bool.Parse(await EnvironmentVariables.EnablePayments))
            user.AccountPlan = DAL.Enums.AccountPlan.Starter;

        await _userRepository.AddUserAsync(user);
    }

    /// <inheritdoc cref="IRegisterService.ActivateUserAccountAsync" />
    public async Task ActivateUserAccountAsync(ActivateUserAccountDto activateUser)
    {
        activateUser.Token = HttpUtility.UrlDecode(activateUser.Token);

        var user = await _userRepository.GetUserByEmailAsync(activateUser.UserEmail)
                   ?? throw new UserNotFoundException();

        if (user.VerifyToken is null)
            throw new Exception("Verify token was not found");

        if (CryptoHelper.GetHash(user.VerifyToken) != activateUser.Token)
            throw new Exception("Wrong activate token");

        user.IsAccountActivated = true;

        await _userRepository.UpdateUserAsync(user);
    }

    /// <inheritdoc cref="IRegisterService.SendVerificationEmailAsync" />
    public async Task SendVerificationEmailAsync(SendVerificationEmailDto sendVerificationEmailDto,
        User? changeEmailUser = null)
    {
        var user = changeEmailUser ?? await _userRepository.GetUserByEmailAsync(sendVerificationEmailDto.UserEmail) ??
            throw new UserNotFoundException();

        if (user.VerifyToken is null)
            throw new Exception("Verify token was not found");

        var hashToken = CryptoHelper.GetHash(user.VerifyToken);

        var verifyEmailUrl = string.Format(VerifyEmailTemplate, sendVerificationEmailDto.RedirectLink,
            HttpUtility.UrlEncode(sendVerificationEmailDto.UserEmail), HttpUtility.UrlEncode(hashToken));

        _notificationModel.Consumer = user.Email;
        _notificationModel.MetaData = new Dictionary<string, string>
            { { FirstName, user.FirstName }, { VerifyEmailAddressLink, verifyEmailUrl } };

        await _notificationQueueService.SendMessageAsync(_notificationModel);
    }
}