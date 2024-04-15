using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Helpers;

namespace PaymentManager.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class CheckOutController : Controller
{
    private readonly IPaymentSessionService _paymentService;
    private readonly PaymentProducer _serviceBus;

    public CheckOutController(PaymentProducer serviceBus,
        IPaymentSessionService paymentService)
    {
        _serviceBus = serviceBus;
        _paymentService = paymentService;
    }

    /// <summary>
    ///     This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken() =>
        TokenParseHelper.GetUserId(User);

    /// <summary>
    ///     This method retrieves the user balance
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("getUserBalance")]
    public async Task<IActionResult> GetUserBalance()
    {
        var products = await _paymentService.GetUserBalanceAsync(GetUserIdFromToken());
        return Ok(products);
    }

    /// <summary>
    ///     This method creates one time payment request for purchase tokens
    /// </summary>
    /// <param name="streamRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("createPayment")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest streamRequest)
    {
        var payment = await _paymentService.CreateOneTimePaymentAsync(GetUserIdFromToken(), streamRequest);
        await _serviceBus.CreateSessionAsync(payment);
        return Ok();
    }

    /// <summary>
    ///     This method creates free subscription for user
    /// </summary>
    /// <param name="signalRModel"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("createFreeSubscription")]
    public async Task<IActionResult> CreateSubscription([FromBody] SignalRModel signalRModel)
    {
        var session = await _paymentService.CreateFreeSubscriptionAsync(signalRModel, GetUserIdFromToken());
        await _serviceBus.CreateFreeSubscriptionAsync(session);
        return Ok();
    }

    /// <summary>
    ///     This method updates current subscription to new one
    /// </summary>
    /// <param name="newPriceId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("upgradeSubscription")]
    public async Task<IActionResult> UpgradeSubscription([FromQuery] string newPriceId)
    {
        await _paymentService.UpgradeSubscriptionAsync(GetUserIdFromToken(), newPriceId);
        return Ok();
    }

    /// <summary>
    ///     This method downgrades current subscription to a lower one
    /// </summary>
    /// <param name="newPriceId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("downgradeSubscription")]
    public async Task<IActionResult> DowngradeSubscription([FromQuery] string newPriceId)
    {
        await _paymentService.DowngradeSubscriptionAsync(GetUserIdFromToken(), newPriceId);
        return Ok();
    }

    /// <summary>
    ///     This method cancels current subscription
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("cancelSubscription")]
    public async Task<IActionResult> CancelSubscription()
    {
        await _paymentService.CancelSubscriptionAsync(GetUserIdFromToken());
        return Ok();
    }
}