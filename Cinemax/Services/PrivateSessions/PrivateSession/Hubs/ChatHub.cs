using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PrivateSession.Hubs;

[Authorize]
public class ChatHub : Hub
{
    protected readonly ILogger<ChatHub> _logger;

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public async Task SendChatMessage(string message, string userName)
    {
        _logger.LogInformation($"Received message {message}");
        await Clients.Others.SendAsync("ReceiveChatMessage", message, userName, DateTime.Now);
    }
}