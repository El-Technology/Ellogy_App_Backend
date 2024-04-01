using Newtonsoft.Json;
using PaymentManager.BLL.Models;
using PaymentManager.DAL.Enums;
using System.Text;

namespace PaymentManager.BLL.Services.HttpServices;
public class UserExternalHttpService
{
    private readonly HttpClient _httpClient;
    public UserExternalHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("UserManager");
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var result = await _httpClient.GetAsync($"api/UserExternal/get-user-by-id?userId={userId}");

        return JsonConvert.DeserializeObject<UserDto>(await result.Content.ReadAsStringAsync());
    }

    public async Task AddStripeCustomerIdAsync(Guid userId, string stripeId)
    {
        var requestObject = new
        {
            UserId = userId,
            StripeId = stripeId
        };
        var jsonRequest = JsonConvert.SerializeObject(requestObject);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var result = await _httpClient.PostAsync($"api/UserExternal/add-stripe-customer-id", content);

        if (!result.IsSuccessStatusCode)
            throw new Exception("Error while adding stripe customer id");

        await Task.CompletedTask;
    }

    public async Task RemoveStripeCustomerIdAsync(Guid userId)
    {
        var result = await _httpClient.DeleteAsync($"api/UserExternal/remove-stripe-customer-id?userId={userId}");
        if (!result.IsSuccessStatusCode)
            throw new Exception("Error while adding stripe customer id");

        await Task.CompletedTask;
    }

    public async Task UpdateTotalPurchasedTokensAsync(Guid userId, int totalPurchasedTokens)
    {
        var result = await _httpClient.GetAsync($"api/UserExternal/update-total-purchased-tokens?userId={userId}&totalPurchasedTokens={totalPurchasedTokens}");
        if (!result.IsSuccessStatusCode)
            throw new Exception("Error while adding stripe customer id");

        await Task.CompletedTask;
    }

    public async Task UpdateAccountPlanAsync(Guid userId, AccountPlan? accountPlan)
    {
        var result = await _httpClient.GetAsync($"api/UserExternal/update-account-plan?userId={userId}&accountPlan={accountPlan}");
        if (!result.IsSuccessStatusCode)
            throw new Exception("Error while updating account plan");

        await Task.CompletedTask;
    }
}