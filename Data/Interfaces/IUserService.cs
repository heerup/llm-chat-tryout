using LlmChatApp.Data.Models;

namespace LlmChatApp.Data.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(string id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User> CreateUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<List<User>> GetAllUsersAsync();
}