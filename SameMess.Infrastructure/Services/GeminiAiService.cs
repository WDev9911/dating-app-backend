using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SameMess.Application.DTOs.Ai;
using SameMess.Application.Interfaces.Services;
using SameMess.Infrastructure.Settings;

namespace SameMess.Infrastructure.Services;

/// <summary>
/// Gọi Google Gemini (generateContent) cho cả gợi ý mở lời lẫn kiểm duyệt.
/// Dùng responseMimeType=application/json để Gemini trả JSON, parse cho chắc.
/// </summary>
public class GeminiAiService : IAiAssistantService
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;
    private readonly AiSettings _settings;

    public GeminiAiService(HttpClient http, IOptions<AiSettings> options)
    {
        _http = http;
        _settings = options.Value;
    }

    public async Task<List<string>> SuggestIcebreakersAsync(string? myBio, string? theirBio, string? theirName)
    {
        var prompt = $$"""
            Bạn là trợ lý cho một app hẹn hò. Dựa vào hồ sơ của hai người, hãy gợi ý 3 câu mở đầu
            trò chuyện bằng TIẾNG VIỆT: ngắn gọn, thân thiện, tự nhiên, cá nhân hóa theo hồ sơ đối phương,
            tránh sáo rỗng kiểu "hi".
            Hồ sơ của tôi: {{Clean(myBio)}}
            Hồ sơ đối phương ({{Clean(theirName)}}): {{Clean(theirBio)}}
            Chỉ trả về JSON đúng dạng: {"suggestions": ["câu 1", "câu 2", "câu 3"]}
            """;

        var json = await GenerateJsonAsync(prompt, temperature: 0.9);
        if (json is null) return new List<string>();

        try
        {
            var doc = JsonSerializer.Deserialize<SuggestionsResponse>(json, JsonOpts);
            return doc?.Suggestions ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task<ModerationResult> ModerateMessageAsync(string content)
    {
        var prompt = $$"""
            Phân loại tin nhắn sau trong ngữ cảnh app hẹn hò. Gắn cờ (flagged=true) nếu có bất kỳ:
            quấy rối, đe dọa/bạo lực, nội dung tình dục tục tĩu, thù ghét/phân biệt, lừa đảo hoặc xin tiền, spam.
            Tin nhắn: "{{Clean(content)}}"
            Chỉ trả về JSON đúng dạng: {"flagged": true hoặc false, "categories": ["loại vi phạm nếu có"]}
            """;

        var json = await GenerateJsonAsync(prompt, temperature: 0);
        if (json is null) return new ModerationResult { IsFlagged = false };

        try
        {
            var doc = JsonSerializer.Deserialize<ModerationResponse>(json, JsonOpts);
            return new ModerationResult
            {
                IsFlagged = doc?.Flagged ?? false,
                Categories = doc?.Categories?.ToArray() ?? Array.Empty<string>(),
            };
        }
        catch
        {
            return new ModerationResult { IsFlagged = false };
        }
    }

    private async Task<string?> GenerateJsonAsync(string prompt, double temperature)
    {
        if (string.IsNullOrWhiteSpace(_settings.GeminiApiKey))
            return null; // chưa cấu hình key → bỏ qua (fail-open)

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.GeminiModel}:generateContent?key={_settings.GeminiApiKey}";

        var body = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new { responseMimeType = "application/json", temperature },
        };

        try
        {
            using var resp = await _http.PostAsJsonAsync(url, body);
            if (!resp.IsSuccessStatusCode)
                return null;

            var payload = await resp.Content.ReadFromJsonAsync<GeminiResponse>(JsonOpts);
            return payload?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
        }
        catch
        {
            return null; // lỗi mạng/API → fail-open
        }
    }

    private static string Clean(string? s) =>
        string.IsNullOrWhiteSpace(s) ? "(chưa có thông tin)" : s.Replace("\"", "'").Trim();

    // ---- DTO nội bộ để parse ----
    private sealed class SuggestionsResponse { public List<string>? Suggestions { get; set; } }
    private sealed class ModerationResponse { public bool Flagged { get; set; } public List<string>? Categories { get; set; } }
    private sealed class GeminiResponse { public List<Candidate>? Candidates { get; set; } }
    private sealed class Candidate { public ContentBlock? Content { get; set; } }
    private sealed class ContentBlock { public List<Part>? Parts { get; set; } }
    private sealed class Part { public string? Text { get; set; } }
}
