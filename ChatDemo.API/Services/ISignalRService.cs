namespace ChatDemo.API.Services
{
    public interface ISignalRService
    {
        Task SendMessageToAllClients(string message);
        Task SendMessageToClient(string message, string connectionId);
    }
}
