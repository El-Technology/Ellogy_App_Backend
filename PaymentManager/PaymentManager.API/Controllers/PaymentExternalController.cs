using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL.Services;

namespace PaymentManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentExternalController : Controller
{
    private readonly WalletExternalService _walletExternalService;
    public PaymentExternalController(WalletExternalService walletExternalService)
    {
        _walletExternalService = walletExternalService;
    }

    [HttpGet("take-service-free")]
    public async Task<IActionResult> TakeServiceFeeAsync([FromQuery] Guid userId, [FromQuery] int feeAmount)
    {
        await _walletExternalService.TakeServiceFeeAsync(userId, feeAmount);
        return Ok();
    }

    [HttpGet("check-if-user-allowed-to-create-request")]
    public async Task<IActionResult> CheckIfUserAllowedToCreateRequest([FromQuery] Guid userId, [FromQuery] int userMinimum)
    {
        var result = await _walletExternalService.CheckIfUserAllowedToCreateRequest(userId, userMinimum);
        return Ok(result);
    }
}
