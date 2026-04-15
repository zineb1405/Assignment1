using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Assignment1.Hubs
{
    public class EventHub : Hub
    {
        public async Task JoinEventGroup(string eventGroup)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, eventGroup);
        }
    }
}