using LlmChatApp.Data.Models;

namespace LlmChatApp.Data.Interfaces;

public interface IConversationService
{
    Task<Conversation?> GetConversationByIdAsync(string id);
    Task<List<Conversation>> GetConversationsByUserIdAsync(string userId);
    Task<Conversation> CreateConversationAsync(Conversation conversation);
    Task<Conversation> UpdateConversationAsync(Conversation conversation);
    Task DeleteConversationAsync(string id);
}