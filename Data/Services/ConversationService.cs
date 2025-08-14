using LlmChatApp.Data.Interfaces;
using LlmChatApp.Data.Models;

namespace LlmChatApp.Data.Services;

public class ConversationService : FileStorageBase, IConversationService
{
    private readonly string _conversationsDirectory;
    private readonly string _indexFilePath;

    public ConversationService()
    {
        _conversationsDirectory = Path.Combine(DataDirectory, "conversations");
        _indexFilePath = Path.Combine(_conversationsDirectory, "index.json");
        EnsureDirectoryExists(_conversationsDirectory);
    }

    public async Task<Conversation?> GetConversationByIdAsync(string id)
    {
        var metaFilePath = Path.Combine(_conversationsDirectory, id, "meta.json");
        return await ReadJsonFileAsync<Conversation>(metaFilePath);
    }

    public async Task<List<Conversation>> GetConversationsByUserIdAsync(string userId)
    {
        var index = await ReadJsonListFileAsync<Conversation>(_indexFilePath);
        return index.Where(c => c.UserId == userId).OrderByDescending(c => c.UpdatedAt).ToList();
    }

    public async Task<Conversation> CreateConversationAsync(Conversation conversation)
    {
        // Create conversation directory
        var conversationDir = Path.Combine(_conversationsDirectory, conversation.Id);
        EnsureDirectoryExists(conversationDir);

        // Save meta data
        var metaFilePath = Path.Combine(conversationDir, "meta.json");
        await WriteJsonFileAsync(metaFilePath, conversation);

        // Update index
        var index = await ReadJsonListFileAsync<Conversation>(_indexFilePath);
        index.Add(conversation);
        await WriteJsonListFileAsync(_indexFilePath, index);

        return conversation;
    }

    public async Task<Conversation> UpdateConversationAsync(Conversation conversation)
    {
        // Update meta data
        var metaFilePath = Path.Combine(_conversationsDirectory, conversation.Id, "meta.json");
        await WriteJsonFileAsync(metaFilePath, conversation);

        // Update index
        var index = await ReadJsonListFileAsync<Conversation>(_indexFilePath);
        var existingIndex = index.FindIndex(c => c.Id == conversation.Id);
        
        if (existingIndex >= 0)
        {
            index[existingIndex] = conversation;
            await WriteJsonListFileAsync(_indexFilePath, index);
        }

        return conversation;
    }

    public async Task DeleteConversationAsync(string id)
    {
        // Remove from index
        var index = await ReadJsonListFileAsync<Conversation>(_indexFilePath);
        index.RemoveAll(c => c.Id == id);
        await WriteJsonListFileAsync(_indexFilePath, index);

        // Delete conversation directory
        var conversationDir = Path.Combine(_conversationsDirectory, id);
        if (Directory.Exists(conversationDir))
        {
            Directory.Delete(conversationDir, true);
        }
    }
}