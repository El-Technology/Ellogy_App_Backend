using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PaymentManager.Common.Constants;
using PaymentManager.Common.Helpers;
using System.Collections.Concurrent;

namespace PaymentManager.BLL.Hubs;

/// <summary>
///     This class is responsible for managing the payment hub
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PaymentHub : Hub
{
    /// <summary>
    ///     This dictionary contains the list of connections with user id
    /// </summary>
    public static readonly ConcurrentDictionary<string, Guid> ListOfConnections = new();

    /// <summary>
    ///     This method checks if the connection id exists
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    public static bool CheckIfConnectionIdExist(string connectionId)
    {
        return ListOfConnections.ContainsKey(connectionId);
    }

    /// <summary>
    ///     This method checks if the user id exists and returns the connections specific to the user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, Guid>> CheckIfUserIdExistAndReturnConnections(Guid userId)
    {
        return ListOfConnections.Where(x => x.Value == userId);
    }

    /// <summary>
    ///     This method is called when a client is connected and removes the connection id from the list of connections
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext()
            ?? throw new Exception("HttpContext is null");

        var userId = TokenParseHelper.GetUserId(httpContext.User);

        if (!ListOfConnections.TryAdd(Context.ConnectionId, userId))
            throw new Exception($"Error while adding connection {Context.ConnectionId} for user {userId}");

        await Clients.Client(Context.ConnectionId).SendAsync(Constants.OnConnectedMethod, Context.ConnectionId);
    }

    /// <summary>
    ///     This method is called when a client is disconnected
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (!ListOfConnections.TryRemove(Context.ConnectionId, out var userId))
            throw new Exception($"Error while removing connection {Context.ConnectionId} for user {userId}");

        await base.OnDisconnectedAsync(exception);
    }
}