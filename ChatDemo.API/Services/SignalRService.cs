using ChatDemo.API.Hubs;
using ChatDemo.API.Hubs.HubInterface;
using Microsoft.AspNetCore.SignalR;

namespace ChatDemo.API.Services
{
    public class SignalRService : ISignalRService
    {
        readonly IHubContext<MyHub,IMessageClient> _hubContext;
        public SignalRService(IHubContext<MyHub,IMessageClient> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendMessageToAllClients(string message)
        {
            await _hubContext.Clients.All.RecieveMessage(message);
        }

        public async Task SendMessageToClient(string message,string connectionId)
        {
            await _hubContext.Clients.Client(connectionId).RecieveMessage(message);
        }

    }
}
