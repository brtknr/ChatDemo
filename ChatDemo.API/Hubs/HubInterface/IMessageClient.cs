using ChatDemo.API.Models;
using ChatDemo.API.ViewModels;

namespace ChatDemo.API.Hubs.HubInterface
{
    public interface IMessageClient
    {
        Task RecieveMessage(string message);
        Task RecieveClients(List<string> clients);
        Task RecieveGroups(List<GroupResponseModel> groups);
        Task RecieveMessagesByGroupId(List<Message> messages);
        Task UserJoined(string connectionId);
        Task UserLeft(string connectionId);
    }
}
