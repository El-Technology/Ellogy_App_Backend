using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Dtos;
using PaymentManager.Common.Helpers;

namespace PaymentManager.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class CustomerController : Controller
{
    private readonly IPaymentCustomerService _paymentCustomerService;
    private readonly PaymentProducer _serviceBus;

    public CustomerController(PaymentProducer serviceBus, IPaymentCustomerService paymentCustomerService)
    {
        _paymentCustomerService = paymentCustomerService;
        _serviceBus = serviceBus;
    }

    /// <summary>
    ///     This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken() =>
        TokenParseHelper.GetUserId(User);

    /// <summary>
    ///     This method creates a customer in stripe portal
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("createCustomer")]
    public async Task<IActionResult> CreateCustomer()
    {
        await _paymentCustomerService.CreateCustomerAsync(GetUserIdFromToken());
        return Ok();
    }

    /// <summary>
    ///     This method updates customer data in stripe portal
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("updateCustomerData")]
    public async Task<IActionResult> UpdateCustomerData()
    {
        await _paymentCustomerService.UpdateCustomerDataAsync(GetUserIdFromToken());
        return Ok();
    }

    /// <summary>
    ///     This method adds a payment method to the customer
    /// </summary>
    /// <param name="createSessionRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("addCustomerPaymentMethod")]
    public async Task<IActionResult> AddCustomerPaymentMethod([FromBody] CreateSessionRequest createSessionRequest)
    {
        var sessionCreateOptions =
            await _paymentCustomerService.AddCustomerPaymentMethodAsync(GetUserIdFromToken(), createSessionRequest);
        await _serviceBus.CreateSessionAsync(sessionCreateOptions);
        return Ok();
    }

    /// <summary>
    ///     This method retrieves all payment methods of the customer
    /// </summary>
    /// <param name="stripePaginationRequestDto"></param>
    /// <returns>Paginated list with all customer payments methods</returns>
    [HttpPost]
    [Route("getAllPaymentMethods")]
    public async Task<IActionResult> GetPaymentMethods([FromBody] StripePaginationRequestDto stripePaginationRequestDto)
    {
        return Ok(await _paymentCustomerService.RetrieveCustomerPaymentMethodsAsync(GetUserIdFromToken(),
            stripePaginationRequestDto));
    }

    /// <summary>
    ///     This method sets the default payment method of the customer
    /// </summary>
    /// <param name="paymentMethodId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("setDefaultPaymentMethod")]
    public async Task<IActionResult> SetDefaultPaymentMethod([FromQuery] string paymentMethodId)
    {
        await _paymentCustomerService.SetDefaultPaymentMethodAsync(GetUserIdFromToken(), paymentMethodId);
        return Ok();
    }

    /// <summary>
    ///     This method retrieves the customer's payment history
    /// </summary>
    /// <param name="stripePaginationRequestDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("getCustomerPayments")]
    public async Task<IActionResult> GetCustomerPayments(
        [FromBody] StripePaginationRequestDto stripePaginationRequestDto)
    {
        return Ok(await _paymentCustomerService.GetCustomerPaymentsAsync(GetUserIdFromToken(),
            stripePaginationRequestDto));
    }

    /// <summary>
    ///     This method retrieves the customer's active subscription
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("getActiveSubscription")]
    public async Task<IActionResult> GetActiveSubscription()
    {
        return Ok(await _paymentCustomerService.GetActiveSubscriptionAsync(GetUserIdFromToken()));
    }

    /// <summary>
    ///     This method updates the customer's subscription preview
    /// </summary>
    /// <param name="newPriceId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("updateSubscriptionPreview")]
    public async Task<IActionResult> UpdateSubscriptionPreview(string newPriceId)
    {
        return Ok(await _paymentCustomerService.UpgradeSubscriptionPreviewAsync(GetUserIdFromToken(), newPriceId));
    }

    /// <summary>
    ///     This method detaches the payment method from the customer
    /// </summary>
    /// <param name="paymentMethodId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("detachPaymentMethod")]
    public async Task<IActionResult> DetachPaymentMethod(string paymentMethodId)
    {
        await _paymentCustomerService.DetachPaymentMethodAsync(paymentMethodId);
        return Ok();
    }

    /// <summary>
    ///     This method deletes the customer
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [Route("deleteCustomer")]
    public async Task<IActionResult> DeleteCustomer()
    {
        await _paymentCustomerService.DeleteCustomerAsync(GetUserIdFromToken());
        return Ok();
    }
}