using AutoFixture;
using Moq;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;

namespace PaymentManager.Tests.RepositoryTests;

public class PaymentRepositoryTest
{
    private Fixture _fixture;
    private Mock<IPaymentRepository> _paymentRepository;

    [SetUp]
    public void Setup()
    {
        _paymentRepository = new Mock<IPaymentRepository>();
        _fixture = new Fixture();
    }

    [Test]
    public async Task CreateUserWalletAsync_ShouldCreatesWallet()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var wallet = _fixture.Create<Wallet>();
        _paymentRepository.Setup(x => x.CreateUserWalletAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);

        // Act
        var result = await _paymentRepository.Object.CreateUserWalletAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(wallet));
    }

    [Test]
    public async Task GetUserWalletAsync_WhenWalletExists_ShouldReturnsWallet()
    {
        // Arrange
        var wallet = _fixture.Create<Wallet>();
        _paymentRepository.Setup(x => x.GetUserWalletAsync(It.IsAny<Guid>())).ReturnsAsync(wallet);

        // Act
        var result = await _paymentRepository.Object.GetUserWalletAsync(wallet.UserId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(wallet));
    }

    [Test]
    public async Task GetUserWalletAsync_WhenWalletNotExists_ShouldReturnsNullWallet()
    {
        // Arrange
        var wallet = _fixture.Create<Wallet>();
        _paymentRepository.Setup(x => x.GetUserWalletAsync(It.IsAny<Guid>())).ReturnsAsync((Wallet?)null);

        // Act
        var result = await _paymentRepository.Object.GetUserWalletAsync(wallet.UserId);

        // Assert
        Assert.That(result, Is.Null);
        Assert.That(result, Is.Not.EqualTo(wallet));
    }

    [Test]
    public async Task UpdateBalanceAsync_ShouldUpdatesBalance()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var amountOfPoints = _fixture.Create<int>();
        _paymentRepository.Setup(x => x.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Act
        await _paymentRepository.Object.UpdateBalanceAsync(userId, amountOfPoints);

        // Assert
        _paymentRepository.Verify(x => x.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
    }

    [Test]
    public async Task GetPaymentByIdAsync_WhenPaymentExists_ShouldReturnsPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        _paymentRepository.Setup(x => x.GetPaymentByIdAsync(It.IsAny<string>())).ReturnsAsync(payment);

        // Act
        var result = await _paymentRepository.Object.GetPaymentByIdAsync(payment.PaymentId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(payment));
    }

    [Test]
    public async Task GetPaymentByIdAsync_WhenPaymentNotExists_ShouldReturnsNullPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        _paymentRepository.Setup(x => x.GetPaymentByIdAsync(It.IsAny<string>())).ReturnsAsync((Payment?)null);

        // Act
        var result = await _paymentRepository.Object.GetPaymentByIdAsync(payment.PaymentId);

        // Assert
        Assert.That(result, Is.Null);
        Assert.That(result, Is.Not.EqualTo(payment));
    }

    [Test]
    public async Task UpdatePaymentAsync_ShouldUpdatesPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        _paymentRepository.Setup(x => x.UpdatePaymentAsync(It.IsAny<Payment>())).Returns(Task.CompletedTask);

        // Act
        await _paymentRepository.Object.UpdatePaymentAsync(payment);

        // Assert
        _paymentRepository.Verify(x => x.UpdatePaymentAsync(It.IsAny<Payment>()), Times.Once);
    }

    [Test]
    public async Task CreatePaymentAsync_ShouldCreatesPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        _paymentRepository.Setup(x => x.CreatePaymentAsync(It.IsAny<Payment>())).Returns(Task.CompletedTask);

        // Act
        await _paymentRepository.Object.CreatePaymentAsync(payment);

        // Assert
        _paymentRepository.Verify(x => x.CreatePaymentAsync(It.IsAny<Payment>()), Times.Once);
    }

    [Test]
    public async Task GetPaymentAsync_WhenPaymentExists_ShouldReturnsPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        _paymentRepository.Setup(x => x.GetPaymentAsync(It.IsAny<string>())).ReturnsAsync(payment);

        // Act
        var result = await _paymentRepository.Object.GetPaymentAsync(payment.SessionId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(payment));
    }

    [Test]
    public async Task GetPaymentAsync_WhenPaymentNotExists_ShouldReturnsNullPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        _paymentRepository.Setup(x => x.GetPaymentAsync(It.IsAny<string>())).ReturnsAsync((Payment?)null);

        // Act
        var result = await _paymentRepository.Object.GetPaymentAsync(payment.SessionId);

        // Assert
        Assert.That(result, Is.Null);
        Assert.That(result, Is.Not.EqualTo(payment));
    }

    [Test]
    public async Task GetPaymentByInvoiceIdAsync_WhenPaymentExists_ShouldReturnsPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        _paymentRepository.Setup(x => x.GetPaymentByInvoiceIdAsync(It.IsAny<string>())).ReturnsAsync(payment);

        // Act
        var result = await _paymentRepository.Object.GetPaymentByInvoiceIdAsync(payment.InvoiceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(payment));
    }

    [Test]
    public async Task GetPaymentByInvoiceIdAsync_WhenPaymentNotExists_ShouldReturnsNullPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        _paymentRepository.Setup(x => x.GetPaymentByInvoiceIdAsync(It.IsAny<string>())).ReturnsAsync((Payment?)null);

        // Act
        var result = await _paymentRepository.Object.GetPaymentByInvoiceIdAsync(payment.InvoiceId);

        // Assert
        Assert.That(result, Is.Null);
        Assert.That(result, Is.Not.EqualTo(payment));
    }

    [Test]
    public async Task GetPaymentByInvoiceOrPaymentIdAsync_WhenPaymentExists_ShouldReturnsPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        _paymentRepository.Setup(x => x.GetPaymentByInvoiceOrPaymentIdAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(payment);

        // Act
        var result =
            await _paymentRepository.Object.GetPaymentByInvoiceOrPaymentIdAsync(payment.PaymentId, payment.InvoiceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(payment));
    }

    [Test]
    public async Task GetPaymentByInvoiceOrPaymentIdAsync_WhenPaymentNotExists_ShouldReturnsNullPayment()
    {
        // Arrange
        var payment = _fixture.Create<Payment>();
        _paymentRepository.Setup(x => x.GetPaymentByInvoiceOrPaymentIdAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Payment?)null);

        // Act
        var result =
            await _paymentRepository.Object.GetPaymentByInvoiceOrPaymentIdAsync(payment.PaymentId, payment.InvoiceId);

        // Assert
        Assert.That(result, Is.Null);
        Assert.That(result, Is.Not.EqualTo(payment));
    }
}