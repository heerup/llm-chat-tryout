using LlmChatApp.Data.Interfaces;
using LlmChatApp.Data.Models;
using System.Security.Cryptography;
using System.Text;

namespace LlmChatApp.Data.Services;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User> RegisterAsync(string username, string password, string role = "User");
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public class AuthService : IAuthService
{
    private readonly IUserService _userService;

    public AuthService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }
        return user;
    }

    public async Task<User> RegisterAsync(string username, string password, string role = "User")
    {
        var existingUser = await _userService.GetUserByUsernameAsync(username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password),
            Role = role
        };

        return await _userService.CreateUserAsync(user);
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "SALT_KEY"));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hash)
    {
        // Generate a unique salt for this user
        var salt = GenerateSalt();
        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password, salt),
            Salt = salt,
            Role = role
        };

        return await _userService.CreateUserAsync(user);
    }

    // Generate a cryptographically secure random salt
    private string GenerateSalt(int size = 16)
    {
        var saltBytes = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    public string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        return HashPassword(password, salt) == hash;
    }
}