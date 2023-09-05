using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AICommunicationService.BLL.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StreamAiHub : Hub
    {
        public static readonly List<string> listOfConnections = new();
        public override async Task OnConnectedAsync()
        {
            listOfConnections.Add(Context.ConnectionId);
            await Clients.All.SendAsync("OnConnected", $"{Context.ConnectionId}");
        }
    }
}
