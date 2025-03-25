using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PrivateSession.DTOs;

namespace PrivateSession.Hubs;

[Authorize]
public class ChatHub : Hub
{
    protected readonly ILogger<ChatHub> _logger;

    private static Dictionary<string, List<Message>> _sessionMessages = new Dictionary<string, List<Message>>();

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public async Task JoinGroup(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        await SendAllMessagesSentSoFar(Context.ConnectionId, sessionId);
    }

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    private void SaveToSessionMessages(string sessionId, string message, string userName)
    {
        if (!_sessionMessages.ContainsKey(sessionId))
        {
            _sessionMessages.Add(sessionId, new List<Message>());
        }

        var chatMessage = new Message()
        {
            message = message,
            date = DateTime.Now,
            username = userName
        };
        _sessionMessages[sessionId].Add(chatMessage);
    }
    public async Task SendChatMessage(string sessionId, string message, string userName)
    {
        _logger.LogInformation($"Received message {message}");
        
        SaveToSessionMessages(sessionId, message, userName);
       
        await Clients.Others.SendAsync("ReceiveChatMessage", message, userName, DateTime.Now);
    }

    private async Task SendAllMessagesSentSoFar(string connectionId, string sessionId)
    {
        if (_sessionMessages.ContainsKey(sessionId))
        {
            var chatMessages = _sessionMessages[sessionId];
            _logger.LogInformation($"Sent {chatMessages.Count} chat messages to {connectionId}");
            await Clients.Client(connectionId).SendAsync("ReceiveAllMessages", chatMessages);
        }
    }
}