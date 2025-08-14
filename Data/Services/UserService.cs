using LlmChatApp.Data.Interfaces;
using LlmChatApp.Data.Models;

namespace LlmChatApp.Data.Services;

public class UserService : FileStorageBase, IUserService
{
    private readonly string _usersFilePath;

    public UserService()
    {
        _usersFilePath = Path.Combine(DataDirectory, "users.json");
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        var users = await ReadJsonListFileAsync<User>(_usersFilePath);
        return users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        var users = await ReadJsonListFileAsync<User>(_usersFilePath);
        return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var users = await ReadJsonListFileAsync<User>(_usersFilePath);
        users.Add(user);
        await WriteJsonListFileAsync(_usersFilePath, users);
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        var users = await ReadJsonListFileAsync<User>(_usersFilePath);
        var existingIndex = users.FindIndex(u => u.Id == user.Id);
        
        if (existingIndex >= 0)
        {
            users[existingIndex] = user;
            await WriteJsonListFileAsync(_usersFilePath, users);
        }
        
        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await ReadJsonListFileAsync<User>(_usersFilePath);
    }
}