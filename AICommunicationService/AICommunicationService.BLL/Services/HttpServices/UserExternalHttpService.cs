using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces.HttpInterfaces;
using AICommunicationService.Common.Dtos;
using Newtonsoft.Json;
using System.Text;

namespace AICommunicationService.BLL.Services.HttpServices;
public class UserExternalHttpService : IUserExternalHttpService
{
    private readonly HttpClient _httpClient;
    public UserExternalHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("UserManager");
    }

    public async Task<List<UserDto>> GetUsersByIdsAsync(List<Guid> userIds)
    {
        var jsonRequest = JsonConvert.SerializeObject(userIds);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var result = await _httpClient.PostAsync("api/UserExternal/get-users-by-ids", content);

        var response = JsonConvert.DeserializeObject<List<UserDto>>(await result.Content.ReadAsStringAsync())
            ?? throw new Exception("Error while getting users by ids");

        return response;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var result = await _httpClient.GetAsync($"api/UserExternal/get-user-by-id?userId={userId}");

        var response = JsonConvert.DeserializeObject<UserDto>(await result.Content.ReadAsStringAsync())
            ?? throw new Exception("Error while getting user by id");

        return response;
    }

    public async Task<List<UserDto>> FindUserByEmailAsync(string email)
    {
        var result = await _httpClient.GetAsync($"api/UserExternal/find-user-by-email?emailPrefix={email}");

        var response = JsonConvert.DeserializeObject<List<UserDto>>(await result.Content.ReadAsStringAsync())
            ?? throw new Exception("Error while finding user by email");

        return response;
    }

    public async Task<PaginationResponseDto<UserDto>> GetUsersByIdsWithPaginationAsync(List<Guid> userIds, PaginationRequestDto paginationRequest)
    {
        var requestObject = new
        {
            UserIds = userIds,
            PaginationRequest = paginationRequest
        };

        var jsonRequest = JsonConvert.SerializeObject(requestObject);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var result = await _httpClient.PostAsync("api/UserExternal/get-users-by-ids-with-pagination", content);

        var response = JsonConvert.DeserializeObject<PaginationResponseDto<UserDto>>(await result.Content.ReadAsStringAsync())
            ?? throw new Exception("Error while getting users by ids with pagination");

        return response;
    }

    public async Task UpdateUserTotalPointsUsageAsync(Guid userId, int usedTokens)
    {
        var requestObject = new
        {
            UserId = userId,
            UsedTokens = usedTokens
        };

        var jsonRequest = JsonConvert.SerializeObject(requestObject);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync($"api/UserExternal/update-user-total-points-usage", content);

        if (!result.IsSuccessStatusCode)
            throw new Exception("Error while updating user total points usage");

        await Task.CompletedTask;
    }

    public async Task<int> GetTotalPointsUsageAsync(Guid userId)
    {
        var result = await _httpClient.GetAsync($"api/UserExternal/get-user-total-points-usage?userId={userId}");

        return JsonConvert.DeserializeObject<int>(await result.Content.ReadAsStringAsync());
    }
}
