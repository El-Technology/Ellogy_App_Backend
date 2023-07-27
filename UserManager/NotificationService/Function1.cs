using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserManager.Common.Models.NotificationModels;
using NotificationService.Interfaces;

namespace NotificationService
{
    public class Function1
    {
        private readonly INotifyService _notifier;

        public Function1(INotifyService notifier)
        {
            _notifier = notifier;
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<NotificationModel>(requestBody);
                log.LogInformation($"Notification: {data}");

                await _notifier.SendNotificationAsync(data);

                return new OkObjectResult(null);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString());
            }
            return null;
        }
    }
}
