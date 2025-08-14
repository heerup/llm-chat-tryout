using Microsoft.AspNetCore.Mvc;
using LlmChatApp.Data.Interfaces;
using LlmChatApp.Data.Models;
using LlmChatApp.Data.Services;
using LlmChatApp.Models;

namespace LlmChatApp.Controllers;

public class ChatController : Controller
{
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly ILlmService _llmService;
    private readonly IQueueService _queueService;

    public ChatController(
        IConversationService conversationService,
        IMessageService messageService,
        ILlmService llmService,
        IQueueService queueService)
    {
        _conversationService = conversationService;
        _messageService = messageService;
        _llmService = llmService;
        _queueService = queueService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var conversations = await _conversationService.GetConversationsByUserIdAsync(userId);
        return View(conversations);
    }

    [HttpGet]
    public async Task<IActionResult> Conversation(string id)
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var conversation = await _conversationService.GetConversationByIdAsync(id);
        if (conversation == null || conversation.UserId != userId)
        {
            return NotFound();
        }

        var messages = await _messageService.GetMessagesByConversationIdAsync(id);
        var viewModel = new ChatViewModel
        {
            Conversation = conversation,
            Messages = messages
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> NewConversation()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var conversation = new Conversation
        {
            UserId = userId,
            Title = "New Conversation"
        };

        await _conversationService.CreateConversationAsync(conversation);
        return RedirectToAction("Conversation", new { id = conversation.Id });
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string conversationId, string message)
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (string.IsNullOrEmpty(message))
        {
            return RedirectToAction("Conversation", new { id = conversationId });
        }

        var conversation = await _conversationService.GetConversationByIdAsync(conversationId);
        if (conversation == null || conversation.UserId != userId)
        {
            return NotFound();
        }

        // Add user message
        var userMessage = new Message
        {
            ConversationId = conversationId,
            Content = message,
            IsFromUser = true
        };
        await _messageService.AddMessageAsync(userMessage);

        // Update conversation title if it's the first message
        if (conversation.Title == "New Conversation")
        {
            conversation.Title = message.Length > 50 ? message.Substring(0, Math.Min(message.Length, 50)) + "..." : message;
            conversation.UpdatedAt = DateTime.UtcNow;
            await _conversationService.UpdateConversationAsync(conversation);
        }

        // Add to queue for LLM processing
        var queueItem = new QueueItem
        {
            UserId = userId,
            ConversationId = conversationId,
            Query = message
        };
        await _queueService.AddToQueueAsync(queueItem);

        // Process immediately if possible (simplified queue)
        await ProcessQueueItem(queueItem);

        return RedirectToAction("Conversation", new { id = conversationId });
    }

    private async Task ProcessQueueItem(QueueItem queueItem)
    {
        try
        {
            // Update queue status
            queueItem.Status = "Processing";
            await _queueService.UpdateQueueItemAsync(queueItem);

            // Get LLM response
            var response = await _llmService.GenerateResponseAsync(queueItem.Query);

            // Add LLM message
            var llmMessage = new Message
            {
                ConversationId = queueItem.ConversationId,
                Content = response,
                IsFromUser = false,
                LLMModel = "llama3.2"
            };
            await _messageService.AddMessageAsync(llmMessage);

            // Update conversation timestamp
            var conversation = await _conversationService.GetConversationByIdAsync(queueItem.ConversationId);
            if (conversation != null)
            {
                conversation.UpdatedAt = DateTime.UtcNow;
                await _conversationService.UpdateConversationAsync(conversation);
            }

            // Mark queue item as completed
            queueItem.Status = "Completed";
            await _queueService.UpdateQueueItemAsync(queueItem);
        }
        catch
        {
            queueItem.Status = "Failed";
            await _queueService.UpdateQueueItemAsync(queueItem);
        }
    }
}