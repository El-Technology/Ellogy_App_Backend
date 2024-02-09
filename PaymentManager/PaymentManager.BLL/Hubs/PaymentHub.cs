using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PaymentManager.Common.Constants;
using PaymentManager.Common.Options;

namespace PaymentManager.BLL.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentHub : Hub
    {
        public static readonly Dictionary<string, Guid> listOfConnections = new();

        public static bool CheckIfConnectionIdExist(string connectionId)
        {
            return listOfConnections.ContainsKey(connectionId);
        }

        public static string? CheckIfUserIdExistAndReturnConnectionId(Guid userId)
        {
            return listOfConnections.FirstOrDefault(x => x.Value == userId).Key;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.User.FindFirst(JwtOptions.UserIdClaimName)?.Value
                ?? throw new Exception($"UserId was not found");

            listOfConnections.Add(Context.ConnectionId, Guid.Parse(userId));
            await Clients.Client(Context.ConnectionId).SendAsync(Constants.OnConnectedMethod, Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            listOfConnections.Remove(Context.ConnectionId);

            await Clients.All.SendAsync("Disconnected", Context.ConnectionId); //for testing, will be removed

            await base.OnDisconnectedAsync(exception);
        }
    }
}
