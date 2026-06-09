using SameMess.Application.DTOs.Reputation;
using SameMess.Application.Interfaces.Services;
using SameMess.Application.Reputation;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class ReputationService : IReputationService
{
    private const int RecentEventsToShow = 10;

    private readonly IReputationEventRepository _eventRepository;
    private readonly IReputationScoreRepository _scoreRepository;

    public ReputationService(
        IReputationEventRepository eventRepository,
        IReputationScoreRepository scoreRepository)
    {
        _eventRepository = eventRepository;
        _scoreRepository = scoreRepository;
    }

    public async Task RecordEventAsync(Guid userId, string eventType, string? reason = null)
    {
        if (!ReputationConfig.Events.TryGetValue(eventType, out var def))
            return;

        // Sự kiện "1 lần" (hoàn thiện hồ sơ, xác minh mặt) — đã có thì bỏ qua
        if (def.Once && await _eventRepository.ExistsAsync(userId, eventType))
            return;

        await _eventRepository.AddAsync(new ReputationEvent
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = eventType,
            Delta = def.Delta,
            Severity = def.Severity,
            Reason = reason,
            CreatedAt = DateTime.UtcNow,
        });
        await _eventRepository.SaveChangesAsync();

        await RecomputeAsync(userId);
    }

    public async Task<ReputationDto> GetMyReputationAsync(Guid userId)
    {
        var events = await _eventRepository.GetByUserAsync(userId);
        var faceVerified = events.Any(e => e.Type == ReputationEventType.FaceVerified);
        var score = ComputeScore(events, faceVerified);
        var tier = ReputationConfig.TierOf(score);

        return new ReputationDto
        {
            Score = score,
            Tier = tier,
            TierLabel = ReputationConfig.TierLabel(tier),
            FaceVerified = faceVerified,
            IsCapped = !faceVerified && score >= ReputationConfig.UnverifiedCap,
            Cap = faceVerified ? ReputationConfig.MaxScore : ReputationConfig.UnverifiedCap,
            RecentEvents = events
                .Take(RecentEventsToShow)
                .Select(e => new ReputationEventDto
                {
                    Type = e.Type,
                    Delta = e.Delta,
                    Severity = e.Severity,
                    Reason = e.Reason,
                    CreatedAt = e.CreatedAt,
                })
                .ToList(),
            HowToImprove = BuildTips(faceVerified),
        };
    }

    public async Task<Dictionary<Guid, int>> GetScoresAsync(IEnumerable<Guid> userIds)
    {
        var ids = userIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, int>();

        var rows = await _scoreRepository.GetByUserIdsAsync(ids);
        var byId = rows.ToDictionary(r => r.UserId, r => r.Score);

        // Thiếu dòng điểm → coi như khởi điểm
        foreach (var id in ids)
            byId.TryAdd(id, ReputationConfig.StartScore);

        return byId;
    }

    /// <summary>Tính lại điểm từ log và lưu bản chụp (upsert ReputationScore).</summary>
    private async Task RecomputeAsync(Guid userId)
    {
        var events = await _eventRepository.GetByUserAsync(userId);
        var faceVerified = events.Any(e => e.Type == ReputationEventType.FaceVerified);
        var score = ComputeScore(events, faceVerified);
        var tier = ReputationConfig.TierOf(score);

        var row = await _scoreRepository.GetByUserAsync(userId);
        if (row is null)
        {
            await _scoreRepository.AddAsync(new ReputationScore
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Score = score,
                Tier = tier,
                UpdatedAt = DateTime.UtcNow,
            });
        }
        else
        {
            row.Score = score;
            row.Tier = tier;
            row.UpdatedAt = DateTime.UtcNow;
        }

        await _scoreRepository.SaveChangesAsync();
    }

    /// <summary>điểm = khởi_điểm + Σ(Delta × hệ_số_phai). Positive/Severe giữ nguyên, Light phai dần.</summary>
    private static int ComputeScore(List<ReputationEvent> events, bool faceVerified)
    {
        double score = ReputationConfig.StartScore;

        // Mọi sự kiện nhẹ cùng neo vào lần vi phạm nhẹ GẦN NHẤT → tái phạm reset đồng hồ
        var lightDates = events
            .Where(e => e.Severity == ReputationSeverity.Light)
            .Select(e => e.CreatedAt)
            .ToList();
        var now = DateTime.UtcNow;

        foreach (var e in events)
        {
            if (e.Severity == ReputationSeverity.Light)
            {
                var lastLight = lightDates.Max();
                var days = (int)Math.Floor((now - lastLight).TotalDays);
                score += e.Delta * ReputationConfig.LightDecayFactor(days);
            }
            else
            {
                // Positive & Severe: giữ nguyên, không phai
                score += e.Delta;
            }
        }

        var clamped = Math.Clamp((int)Math.Round(score), ReputationConfig.MinScore, ReputationConfig.MaxScore);

        // Chưa xác minh khuôn mặt → đụng trần
        if (!faceVerified && clamped > ReputationConfig.UnverifiedCap)
            clamped = ReputationConfig.UnverifiedCap;

        return clamped;
    }

    private static List<string> BuildTips(bool faceVerified)
    {
        var tips = new List<string>();
        if (!faceVerified)
            tips.Add("Xác minh khuôn mặt để +15 điểm và gỡ trần 65 (lên Uy tín tốt/cao).");
        tips.Add("Hoàn thiện hồ sơ và trò chuyện lành mạnh để tăng điểm.");
        tips.Add("Tránh bị report/block — vi phạm nặng sẽ trừ nhiều và không tự hồi phục.");
        return tips;
    }
}
