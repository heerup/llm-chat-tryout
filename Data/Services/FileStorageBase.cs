using Newtonsoft.Json;

namespace LlmChatApp.Data.Services;

public class FileStorageBase
{
    protected readonly string DataDirectory;

    public FileStorageBase()
    {
        DataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data");
        EnsureDirectoryExists(DataDirectory);
    }

    protected void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    protected async Task<T?> ReadJsonFileAsync<T>(string filePath) where T : class
    {
        if (!File.Exists(filePath))
            return null;

        var json = await File.ReadAllTextAsync(filePath);
        return JsonConvert.DeserializeObject<T>(json);
    }

    protected async Task WriteJsonFileAsync<T>(string filePath, T data)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            EnsureDirectoryExists(directory);
        }

        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        await File.WriteAllTextAsync(filePath, json);
    }

    protected async Task<List<T>> ReadJsonListFileAsync<T>(string filePath) where T : class
    {
        if (!File.Exists(filePath))
            return new List<T>();

        var json = await File.ReadAllTextAsync(filePath);
        return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
    }

    protected async Task WriteJsonListFileAsync<T>(string filePath, List<T> data)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            EnsureDirectoryExists(directory);
        }

        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        await File.WriteAllTextAsync(filePath, json);
    }
}