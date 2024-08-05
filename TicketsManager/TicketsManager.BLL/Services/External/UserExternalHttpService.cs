using Newtonsoft.Json;
using System.Text;
using TicketsManager.BLL.Dtos;
using TicketsManager.BLL.Interfaces.External;

namespace TicketsManager.BLL.Services.External;
public class UserExternalHttpService : IUserExternalHttpService
{
    private readonly HttpClient _httpClient;
    public UserExternalHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("UserManager");
    }

    public async Task<List<UserDto>> FindUserByEmailAsync(string email)
    {
        var result = await _httpClient.GetAsync($"api/UserExternal/find-user-by-email?emailPrefix={email}");

        var response = JsonConvert.DeserializeObject<List<UserDto>>(await result.Content.ReadAsStringAsync())
            ?? throw new Exception("Error while finding user by email");

        return response;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var result = await _httpClient.GetAsync($"api/UserExternal/get-user-by-id?userId={userId}");

        var response = JsonConvert.DeserializeObject<UserDto>(await result.Content.ReadAsStringAsync())
            ?? throw new Exception("Error while getting user by id");

        return response;
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
}