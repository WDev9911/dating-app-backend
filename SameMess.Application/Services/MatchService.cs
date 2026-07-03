using SameMess.Application.Common;
using SameMess.Application.DTOs.Matching;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;

    public MatchService(IMatchRepository matchRepository, IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
    }

    public async Task<List<MatchDto>> GetMyMatchesAsync(Guid userId)
    {
        var matches = await _matchRepository.GetActiveForUserAsync(userId);
        if (matches.Count == 0)
            return new List<MatchDto>();

        // Lấy info của những người đối diện trong một truy vấn
        var otherIds = matches
            .Select(m => m.UserAId == userId ? m.UserBId : m.UserAId)
            .ToList();

        var others = await _userRepository.GetWithProfileAndPhotosByIdsAsync(otherIds);
        var byId = others.ToDictionary(u => u.Id);

        var result = new List<MatchDto>();
        foreach (var match in matches)
        {
            var otherId = match.UserAId == userId ? match.UserBId : match.UserAId;
            byId.TryGetValue(otherId, out var other);

            result.Add(new MatchDto
            {
                MatchId = match.Id,
                UserId = otherId,
                DisplayName = other?.Profile?.DisplayName ?? string.Empty,
                AvatarUrl = AvatarOf(other),
                Age = AgeCalculator.FromDateOfBirth(other?.Profile?.DateOfBirth),
                MatchedAt = match.CreatedAt,
                IsAdmin = other?.Role == UserRole.Admin,
                AvatarFrame = other?.Profile?.AvatarFrame,
            });
        }

        return result;
    }

    /// <summary>Avatar = AvatarUrl của hồ sơ; nếu trống thì lấy ảnh chính (hoặc ảnh đầu).</summary>
    internal static string? AvatarOf(User? user)
    {
        if (!string.IsNullOrEmpty(user?.Profile?.AvatarUrl)) return user.Profile.AvatarUrl;
        var photos = user?.Photos;
        if (photos is null || photos.Count == 0) return null;
        return (photos.FirstOrDefault(p => p.IsPrimary) ?? photos.First()).Url;
    }

    public async Task UnmatchAsync(Guid userId, Guid matchId)
    {
        var match = await _matchRepository.GetByIdForUserAsync(matchId, userId)
            ?? throw new NotFoundException("Match", matchId);

        match.IsActive = false;
        await _matchRepository.SaveChangesAsync();
    }
}
