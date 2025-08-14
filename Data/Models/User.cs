namespace LlmChatApp.Data.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User"; // User or Admin
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}