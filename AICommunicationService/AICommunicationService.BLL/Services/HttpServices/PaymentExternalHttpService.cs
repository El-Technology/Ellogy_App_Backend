using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces.HttpInterfaces;
using Newtonsoft.Json;
using System.Text;

namespace AICommunicationService.BLL.Services.HttpServices;
public class PaymentExternalHttpService : IPaymentExternalHttpService
{
    private readonly HttpClient _httpClient;
    public PaymentExternalHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PaymentManager");
    }

    public async Task<List<UserDto>> GetUsersByIdsAsync(List<Guid> userIds)
    {
        var jsonRequest = JsonConvert.SerializeObject(userIds);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var result = await _httpClient.PostAsync("api/UserExternal/get-users-by-ids", content);
        return JsonConvert.DeserializeObject<List<UserDto>>(await result.Content.ReadAsStringAsync());
    }
}
