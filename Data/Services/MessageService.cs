using LlmChatApp.Data.Interfaces;
using LlmChatApp.Data.Models;

namespace LlmChatApp.Data.Services;

public class MessageService : FileStorageBase, IMessageService
{
    private readonly string _conversationsDirectory;

    public MessageService()
    {
        _conversationsDirectory = Path.Combine(DataDirectory, "conversations");
        EnsureDirectoryExists(_conversationsDirectory);
    }

    public async Task<List<Message>> GetMessagesByConversationIdAsync(string conversationId)
    {
        var messagesFilePath = Path.Combine(_conversationsDirectory, conversationId, "messages.json");
        return await ReadJsonListFileAsync<Message>(messagesFilePath);
    }

    public async Task<Message> AddMessageAsync(Message message)
    {
        var conversationDir = Path.Combine(_conversationsDirectory, message.ConversationId);
        EnsureDirectoryExists(conversationDir);

        var messagesFilePath = Path.Combine(conversationDir, "messages.json");
        var messages = await ReadJsonListFileAsync<Message>(messagesFilePath);
        
        messages.Add(message);
        await WriteJsonListFileAsync(messagesFilePath, messages);

        return message;
    }

    public async Task<Message?> GetMessageByIdAsync(string id)
    {
        // Search through all conversation directories
        if (!Directory.Exists(_conversationsDirectory))
            return null;

        var conversationDirs = Directory.GetDirectories(_conversationsDirectory);
        
        foreach (var dir in conversationDirs)
        {
            var messagesFilePath = Path.Combine(dir, "messages.json");
            var messages = await ReadJsonListFileAsync<Message>(messagesFilePath);
            var message = messages.FirstOrDefault(m => m.Id == id);
            if (message != null)
                return message;
        }

        return null;
    }
}