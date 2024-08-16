namespace ChatDemo.API.Hubs.HubInterface
{
    public interface IMessageClient
    {
        Task RecieveMessage(string message);
        Task RecieveClients(List<string> clients);
        Task UserJoined(string connectionId);
        Task UserLeft(string connectionId);
    }
}
