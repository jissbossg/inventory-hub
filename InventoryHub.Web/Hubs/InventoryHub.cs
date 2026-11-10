using Microsoft.AspNetCore.SignalR;

namespace InventoryHub.Web.Hubs;

public class InventoryHubHub : Hub
{
    public Task JoinInventoryGroup() => Groups.AddToGroupAsync(Context.ConnectionId, "inventory");
}