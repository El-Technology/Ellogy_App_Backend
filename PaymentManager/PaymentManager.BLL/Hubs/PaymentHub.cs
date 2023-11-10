using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PaymentManager.Common.Constants;

namespace PaymentManager.BLL.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentHub : Hub
    {
        public static readonly List<string> listOfConnections = new();
        public override async Task OnConnectedAsync()
        {
            listOfConnections.Add(Context.ConnectionId);
            await Clients.All.SendAsync(Constants.OnConnectedMethod, Context.ConnectionId);
        }
    }
}
