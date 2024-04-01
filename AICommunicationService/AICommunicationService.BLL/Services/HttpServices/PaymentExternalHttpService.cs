using AICommunicationService.BLL.Interfaces.HttpInterfaces;
using Newtonsoft.Json;

namespace AICommunicationService.BLL.Services.HttpServices;
public class PaymentExternalHttpService : IPaymentExternalHttpService
{
    private readonly HttpClient _httpClient;
    public PaymentExternalHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PaymentManager");
    }

    public async Task TakeServiceFeeAsync(Guid userId, int feeAmount)
    {
        var result = await _httpClient.GetAsync($"api/PaymentExternal/take-service-free?userId={userId}&feeAmount={feeAmount}");

        if (!result.IsSuccessStatusCode)
            throw new Exception("Failed to take service fee");

        await Task.CompletedTask;
    }

    public async Task<bool> CheckIfUserAllowedToCreateRequestAsync(Guid userId, int userMinimum)
    {
        var result = await _httpClient.GetAsync($"api/PaymentExternal/check-if-user-allowed-to-create-request?userId={userId}&userMinimum={userMinimum}");
        return JsonConvert.DeserializeObject<bool>(await result.Content.ReadAsStringAsync());
    }

}
