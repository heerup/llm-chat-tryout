using LlmChatApp.Data.Interfaces;
using LlmChatApp.Data.Models;

namespace LlmChatApp.Data.Services;

public class QueueService : FileStorageBase, IQueueService
{
    private readonly string _queueDirectory;
    private readonly string _activeQueueFilePath;

    public QueueService()
    {
        _queueDirectory = Path.Combine(DataDirectory, "queue");
        _activeQueueFilePath = Path.Combine(_queueDirectory, "active.json");
        EnsureDirectoryExists(_queueDirectory);
    }

    public async Task<QueueItem> AddToQueueAsync(QueueItem item)
    {
        var queue = await ReadJsonListFileAsync<QueueItem>(_activeQueueFilePath);
        queue.Add(item);
        await WriteJsonListFileAsync(_activeQueueFilePath, queue);
        return item;
    }

    public async Task<QueueItem?> GetNextQueueItemAsync()
    {
        var queue = await ReadJsonListFileAsync<QueueItem>(_activeQueueFilePath);
        return queue
            .Where(q => q.Status == "Pending")
            .OrderBy(q => q.CreatedAt)
            .FirstOrDefault();
    }

    public async Task UpdateQueueItemAsync(QueueItem item)
    {
        var queue = await ReadJsonListFileAsync<QueueItem>(_activeQueueFilePath);
        var existingIndex = queue.FindIndex(q => q.Id == item.Id);
        
        if (existingIndex >= 0)
        {
            queue[existingIndex] = item;
            await WriteJsonListFileAsync(_activeQueueFilePath, queue);
        }
    }

    public async Task<List<QueueItem>> GetQueueByUserIdAsync(string userId)
    {
        var queue = await ReadJsonListFileAsync<QueueItem>(_activeQueueFilePath);
        return queue
            .Where(q => q.UserId == userId)
            .OrderBy(q => q.CreatedAt)
            .ToList();
    }

    public async Task<int> GetQueuePositionAsync(string queueItemId)
    {
        var queue = await ReadJsonListFileAsync<QueueItem>(_activeQueueFilePath);
        var pendingItems = queue
            .Where(q => q.Status == "Pending")
            .OrderBy(q => q.CreatedAt)
            .ToList();

        var index = pendingItems.FindIndex(q => q.Id == queueItemId);
        return index >= 0 ? index + 1 : -1;
    }
}