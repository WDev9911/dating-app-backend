namespace SameMess.Infrastructure.Settings;

public class AiSettings
{
    public string GeminiApiKey { get; set; } = string.Empty;
    public string GeminiModel { get; set; } = "gemini-2.0-flash-lite";
    public string OpenAiApiKey { get; set; } = string.Empty;
}
