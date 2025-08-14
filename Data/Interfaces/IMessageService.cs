using LlmChatApp.Data.Models;

namespace LlmChatApp.Data.Interfaces;

public interface IMessageService
{
    Task<List<Message>> GetMessagesByConversationIdAsync(string conversationId);
    Task<Message> AddMessageAsync(Message message);
    Task<Message?> GetMessageByIdAsync(string id);
}