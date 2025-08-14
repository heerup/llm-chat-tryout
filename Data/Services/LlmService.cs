using Newtonsoft.Json;

namespace LlmChatApp.Data.Services;

public interface ILlmService
{
    Task<string> GenerateResponseAsync(string prompt, string model = "granite3.1-moe:1b");
    Task<bool> IsOllamaAvailableAsync();
    Task<List<string>> GetAvailableModelsAsync();
}

public class LlmService : ILlmService
{
    private readonly HttpClient _httpClient;
    private readonly string _ollamaBaseUrl;

    public LlmService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _ollamaBaseUrl = configuration.GetValue<string>("Ollama:BaseUrl") ?? "http://localhost:11434";
    }

    public async Task<string> GenerateResponseAsync(string prompt, string model = "granite3.1-moe:1b")
    {
        try
        {
            var request = new
            {
                model = model,
                prompt = prompt,
                stream = false
            };

            var jsonContent = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_ollamaBaseUrl}/api/generate", content);
            
            if (!response.IsSuccessStatusCode)
            {
                return $"Error: Unable to get response from LLM (Status: {response.StatusCode})";
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseJson);

            return result?.response?.ToString() ?? "Error: Invalid response format";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while generating LLM response.");
            return "Error: Unable to generate response due to an internal error.";
        }
    }

    public async Task<bool> IsOllamaAvailableAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_ollamaBaseUrl}/api/tags");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<string>> GetAvailableModelsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_ollamaBaseUrl}/api/tags");
            
            if (!response.IsSuccessStatusCode)
            {
                return new List<string>();
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseJson);

            var models = new List<string>();
            if (result?.models != null)
            {
                foreach (var model in result.models)
                {
                    models.Add(model.name?.ToString() ?? "unknown");
                }
            }

            return models;
        }
        catch
        {
            return new List<string>();
        }
    }
}