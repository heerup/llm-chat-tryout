using LlmChatApp.Data.Models;

namespace LlmChatApp.Data.Interfaces;

public interface IQueueService
{
    Task<QueueItem> AddToQueueAsync(QueueItem item);
    Task<QueueItem?> GetNextQueueItemAsync();
    Task UpdateQueueItemAsync(QueueItem item);
    Task<List<QueueItem>> GetQueueByUserIdAsync(string userId);
    Task<int> GetQueuePositionAsync(string queueItemId);
}