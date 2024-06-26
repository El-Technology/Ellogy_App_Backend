using Newtonsoft.Json;
using System.Text;
using UserManager.BLL.Interfaces;
using UserManager.Common.Models.NotificationModels;

namespace UserManager.BLL.Services.HttpServices;
public class ExternalNotificationService : IExternalNotificationService
{
    private readonly HttpClient _httpClient;
    public ExternalNotificationService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("NotificationManager");
    }

    public async Task SendNotificationAsync(NotificationModel notificationModel)
    {
        var jsonRequest = JsonConvert.SerializeObject(notificationModel);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var result = await _httpClient.PostAsync($"api/ExternalNotification/sendNotification", content);

        if (!result.IsSuccessStatusCode)
            throw new Exception("Error while sending the notification");

        await Task.CompletedTask;
    }
}
