using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace EmployeeManagementSystem.Infrastructure.Services;

// Service implementation for Gemini AI integration
public class GeminiAIService : IAIService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiAIService> _logger;
    private readonly HttpClient _httpClient;

    public GeminiAIService(IConfiguration configuration, ILogger<GeminiAIService> logger, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<string> ProcessQueryAsync(string query)
    {
        try
        {
            var apiKey = _configuration["AISettings:ApiKey"];
            var model = _configuration["AISettings:Model"] ?? "gemini-2.0-flash-exp";
            
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Gemini API Key not configured");
                return "Sorry, the AI ​​service is not configured correctly.";
            }

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"You are a human resources assistant for TalentoPlus S.A.S. Answer the following question concisely and professionally: {query}" }
                        }
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error en Gemini API: {Error}", error);
                return "Sorry, there was an error processing your query.";
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

            var textResponse = jsonResponse
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return textResponse ?? "No response could be generated.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AI query");
            return "Sorry, there was an error processing your query.";
        }
    }
}

