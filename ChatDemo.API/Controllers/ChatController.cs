using ChatDemo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatDemo.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ChatController> _logger;
        private readonly ISignalRService _signalRService;

        public ChatController(ILogger<ChatController> logger,ISignalRService signalRService)
        {
            _logger = logger;
            _signalRService = signalRService;
        }

        [HttpPost]
        public void SendMessageToAllClients(string message)
        {
            _signalRService.SendMessageToAllClients(message);
        }
    }
}
