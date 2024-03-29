using AutoFixture;
using Azure.Messaging.ServiceBus;
using Moq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using UserManager.BLL.Dtos.LoginDtos;
using UserManager.BLL.Dtos.PasswordDtos;
using UserManager.BLL.Dtos.RegisterDtos;
using UserManager.BLL.Services;
using UserManager.Common.Models.NotificationModels;
using UserManager.DAL.Repositories;

namespace UserManager.Tests.ServiceTests;

[TestFixture]
public class PasswordServiceTest : BaseClassForServices
{

    [Test]
    public async Task ForgotPasswordAsync_SendsEmailToUser()
    {
        var userRepository = new UserRepository(_userManagerDbContext);
        var forgotPasswordRepository = new ForgotPasswordRepository(_userManagerDbContext);

        //Create mocks for service bus client and sender
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var serviceBusSenderMock = new Mock<ServiceBusSender>();
        serviceBusClientMock.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSenderMock.Object);
        var notificationQueueService = new NotificationQueueService(serviceBusClientMock.Object);

        //Register new user
        var user = await RegisterNewUserAsync(userRepository);

        //Input fields for recover the password
        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = user.Email,
            RedirectUrl = "someRedirectUrl"
        };

        var passwordService = new PasswordService(userRepository, forgotPasswordRepository, notificationQueueService);
        await passwordService.ForgotPasswordAsync(forgotPasswordDto);

        //Verify if message was sent one times
        serviceBusSenderMock.Verify(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task ResetPasswordAsync_LoginWithNewPassword()
    {
        var sentMessage = new ServiceBusMessage();
        var userRepository = new UserRepository(_userManagerDbContext);
        var forgotPasswordRepository = new ForgotPasswordRepository(_userManagerDbContext);

        //Register new user
        var user = await RegisterNewUserAsync(userRepository);

        //Create notification service queue
        var serviceBusClientMock = new Mock<ServiceBusClient>();
        var serviceBusSenderMock = new Mock<ServiceBusSender>();
        serviceBusSenderMock
            .Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), CancellationToken.None))
            .Callback<ServiceBusMessage, CancellationToken>((message, cancellationToken) =>
            {
                sentMessage = message;
            })
            .Returns(Task.CompletedTask); //Writes sent message in variable
        serviceBusClientMock.Setup(x => x.CreateSender(It.IsAny<string>())).Returns(serviceBusSenderMock.Object);
        var notificationQueueService = new NotificationQueueService(serviceBusClientMock.Object);

        //Send recover the message
        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = user.Email,
            RedirectUrl = "someRedirectUrl"
        };
        var passwordService = new PasswordService(userRepository, forgotPasswordRepository, notificationQueueService);
        await passwordService.ForgotPasswordAsync(forgotPasswordDto);

        //Reset password
        var newPassword = "NewPassword";
        var resetPasswordDto = GetResetPasswordDto(sentMessage.Body, newPassword);
        await passwordService.ResetPasswordAsync(resetPasswordDto);

        //Try to login with new password
        var actualResponse = await TryToLoginAsync(userRepository, user.Email, newPassword);

        Assert.Multiple(() =>
        {
            Assert.That(actualResponse.Jwt, Is.Not.Null);
            Assert.That(actualResponse.RefreshToken, Is.Not.Null);
            Assert.That(actualResponse.Email, Is.EqualTo(user.Email));
        });
    }

    private async Task<UserRegisterRequestDto> RegisterNewUserAsync(UserRepository userRepository)
    {
        //var registerService = new RegisterService(_mapper, userRepository);
        var user = _fixture.Create<UserRegisterRequestDto>();
        //await registerService.RegisterUserAsync(user);
        return user;
    }

    private ResetPasswordDto GetResetPasswordDto(BinaryData message, string newPassword)
    {
        var messageBodyString = Encoding.UTF8.GetString(message);
        var objectMessageBody = JsonSerializer.Deserialize<NotificationModel>(messageBodyString);
        var token = objectMessageBody?.MetaData.FirstOrDefault().Value;

        var regex = new Regex(@"\bid=([a-fA-F0-9-]+)&token=([a-fA-F0-9]+)\b");
        var match = regex.Match(token);

        //Reset input fields
        var resetPasswordDto = new ResetPasswordDto
        {
            Id = Guid.Parse(match.Groups[1].Value),
            Password = newPassword,
            Token = match.Groups[2].Value
        };

        return resetPasswordDto;
    }

    private async Task<LoginResponseDto> TryToLoginAsync(UserRepository userRepository, string email, string newPassword)
    {
        var refreshTokenRepository = new RefreshTokenRepository(_userManagerDbContext);
        var refreshTokenService = new RefreshTokenService(refreshTokenRepository, userRepository);
        var loginService = new LoginService(userRepository, _mapper, refreshTokenService);

        //Login input fields
        var loginRequestDto = new LoginRequestDto
        {
            Email = email,
            Password = newPassword
        };

        return await loginService.LoginUserAsync(loginRequestDto);
    }
}
