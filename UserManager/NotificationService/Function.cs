using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using NotificationService.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManager.Common.Models.NotificationModels;

namespace NotificationService;

public class Function
{
    private readonly INotifyService _notifier;

    public Function(INotifyService notifier)
    {
        _notifier = notifier;
    }

    [FunctionName("HttpFunction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "projectStarted")] HttpRequest req)
    {
        await _notifier.SendNotificationAsync(new NotificationModel
        {
            Type = NotificationTypeEnum.VerifyEmail,
            Way = NotificationWayEnum.Email,
            MetaData = new Dictionary<string, string>
            { { "{{{firstName}}}", "sharkovskiy1@gmail.com" }, { "{{{verifyEmailAddressLink}}}", "https://string.com" } },
            Consumer = "sharkovskiy1@gmail.com"
        });

        return new OkResult();
    }
}
