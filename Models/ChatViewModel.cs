using LlmChatApp.Data.Models;

namespace LlmChatApp.Models;

public class ChatViewModel
{
    public Conversation Conversation { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
    public string NewMessage { get; set; } = string.Empty;
}