using ChatDemo.API.Hubs.HubInterface;
using Microsoft.AspNetCore.SignalR;

namespace ChatDemo.API.Hubs
{
    public class MyHub : Hub<IMessageClient>
    {
        static List<string> clients = new List<string>();
        public async Task ClientToClientSendMessage(string message,string targetConnId)
        {
            //await Clients.Client(targetConnId).SendAsync("recieveMessage",message);
            await Clients.Client(targetConnId).RecieveMessage(message);
        }

        public override async Task OnConnectedAsync()
        {
            clients.Add(Context.ConnectionId);

            //await Clients.All.SendAsync("recieveClients", clients); // old 
            await Clients.All.RecieveClients(clients);               // strongly typed
            await Clients.All.UserJoined(Context.ConnectionId);
            
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            clients.Remove(Context.ConnectionId);
            await Clients.All.RecieveClients(clients);
            await Clients.All.UserLeft(Context.ConnectionId);
        }
        
    }
}
