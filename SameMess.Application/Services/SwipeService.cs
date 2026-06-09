using AutoMapper;
using SameMess.Application.Common;
using SameMess.Application.DTOs.Matching;
using SameMess.Application.DTOs.Profile;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class SwipeService : ISwipeService
{
    private readonly ISwipeRepository _swipeRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBlockRepository _blockRepository;
    private readonly ITaskService _taskService;
    private readonly IReputationService _reputationService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public SwipeService(
        ISwipeRepository swipeRepository,
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IBlockRepository blockRepository,
        ITaskService taskService,
        IReputationService reputationService,
        ISubscriptionService subscriptionService,
        INotificationService notificationService,
        IMapper mapper)
    {
        _swipeRepository = swipeRepository;
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _blockRepository = blockRepository;
        _taskService = taskService;
        _reputationService = reputationService;
        _subscriptionService = subscriptionService;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<SwipeResultDto> SwipeAsync(Guid userId, SwipeRequestDto dto)
    {
        if (dto.TargetUserId == userId)
            throw new ConflictException("You cannot swipe yourself.");

        _ = await _userRepository.GetByIdAsync(dto.TargetUserId)
            ?? throw new NotFoundException("User", dto.TargetUserId);

        if (await _blockRepository.ExistsBetweenAsync(userId, dto.TargetUserId))
            throw new ForbiddenException("You cannot swipe a user you have blocked or who has blocked you.");

        if (await _swipeRepository.GetAsync(userId, dto.TargetUserId) is not null)
            throw new ConflictException("You have already swiped this user.");

        // Gói Free bị giới hạn số Like/SuperLike mỗi ngày; Plus/Gold không giới hạn
        if (SwipeAction.IsLike(dto.Action))
        {
            var ent = await _subscriptionService.GetEntitlementsAsync(userId);
            if (!ent.UnlimitedLikes)
            {
                var since = DateTime.UtcNow.Date; // mốc đầu ngày (UTC)
                var likesToday = await _swipeRepository.CountLikesSinceAsync(userId, since);
                if (likesToday >= ent.DailyLikeLimit)
                    throw new BadRequestException(
                        $"Bạn đã hết {ent.DailyLikeLimit} lượt thích hôm nay. Nâng cấp Plus/Gold để thích không giới hạn.");
            }
        }

        await _swipeRepository.AddAsync(new Swipe
        {
            Id = Guid.NewGuid(),
            SwiperId = userId,
            TargetUserId = dto.TargetUserId,
            Action = dto.Action,
            CreatedAt = DateTime.UtcNow,
        });

        Guid? matchId = null;

        // Chỉ Like/SuperLike mới có thể tạo match — và chỉ khi đối phương đã Like mình từ trước
        if (SwipeAction.IsLike(dto.Action))
        {
            var reverse = await _swipeRepository.GetAsync(dto.TargetUserId, userId);
            if (reverse is not null && SwipeAction.IsLike(reverse.Action))
                matchId = await EnsureMatchAsync(userId, dto.TargetUserId);
        }

        await _swipeRepository.SaveChangesAsync();

        // Gamification (fail-safe): tiến độ "quẹt" + nếu có match thì tiến độ "match"
        try
        {
            await _taskService.RecordActionAsync(userId, GameAction.Swipe);
            if (matchId is not null)
            {
                // Cả hai bên đều được tính "có match"
                await _taskService.RecordActionAsync(userId, GameAction.Match);
                await _taskService.RecordActionAsync(dto.TargetUserId, GameAction.Match);
                await _reputationService.RecordEventAsync(userId, ReputationEventType.GotMatch);
                await _reputationService.RecordEventAsync(dto.TargetUserId, ReputationEventType.GotMatch);
            }
        }
        catch { /* không để gamification/uy tín làm hỏng luồng swipe */ }

        // Thông báo (fail-safe)
        try
        {
            if (matchId is not null)
            {
                await _notificationService.NotifyAsync(userId, NotificationType.Match,
                    "Match mới! 🎉", "Bạn vừa có một match mới.", matchId.ToString());
                await _notificationService.NotifyAsync(dto.TargetUserId, NotificationType.Match,
                    "Match mới! 🎉", "Bạn vừa có một match mới.", matchId.ToString());
            }
            else if (dto.Action == SwipeAction.SuperLike)
            {
                await _notificationService.NotifyAsync(dto.TargetUserId, NotificationType.SuperLike,
                    "Bạn nhận được SuperLike ⭐", "Có người vừa SuperLike bạn.", userId.ToString());
            }
        }
        catch { /* không để thông báo làm hỏng luồng swipe */ }

        return new SwipeResultDto { IsMatch = matchId is not null, MatchId = matchId };
    }

    public async Task<List<LikedMeProfileDto>> GetWhoLikedMeAsync(Guid userId)
    {
        var likers = await _swipeRepository.GetLikersAsync(userId);
        // Ảnh "ai đã like tôi" chỉ mở cho Gold; Free thấy danh sách nhưng ảnh bị khóa
        var ent = await _subscriptionService.GetEntitlementsAsync(userId);
        return await BuildLikerCardsAsync(userId, likers, revealPhotos: ent.CanSeeLikedMePhotos);
    }

    public async Task<List<LikedMeProfileDto>> GetWhoSuperLikedMeAsync(Guid userId)
    {
        var superLikers = await _swipeRepository.GetSuperLikersAsync(userId);
        // SuperLike: người đó chủ động lộ diện để được phản hồi → luôn hiện ảnh
        return await BuildLikerCardsAsync(userId, superLikers, revealPhotos: true);
    }

    private async Task<List<LikedMeProfileDto>> BuildLikerCardsAsync(Guid userId, List<Swipe> swipes, bool revealPhotos)
    {
        if (swipes.Count == 0)
            return new List<LikedMeProfileDto>();

        // Ẩn người có quan hệ block (2 chiều)
        var blockedRelated = (await _blockRepository.GetRelatedUserIdsAsync(userId)).ToHashSet();
        var visible = swipes.Where(s => !blockedRelated.Contains(s.SwiperId)).ToList();
        if (visible.Count == 0)
            return new List<LikedMeProfileDto>();

        var users = await _userRepository.GetWithProfileAndPhotosByIdsAsync(visible.Select(s => s.SwiperId));
        var byId = users.ToDictionary(u => u.Id);

        var result = new List<LikedMeProfileDto>();
        foreach (var swipe in visible)
        {
            if (!byId.TryGetValue(swipe.SwiperId, out var user) || user.Profile is null)
                continue;

            result.Add(new LikedMeProfileDto
            {
                UserId = user.Id,
                DisplayName = user.Profile.DisplayName,
                Age = AgeCalculator.FromDateOfBirth(user.Profile.DateOfBirth),
                Gender = user.Profile.Gender,
                Bio = user.Profile.Bio,
                IsSuperLike = swipe.Action == SwipeAction.SuperLike,
                PhotosLocked = !revealPhotos,
                Photos = revealPhotos
                    ? _mapper.Map<List<PhotoDto>>(user.Photos.OrderBy(p => p.OrderIndex))
                    : new List<PhotoDto>(), // KHÔNG gửi URL gốc cho Free
            });
        }

        return result;
    }

    public async Task<UndoResultDto> UndoLastSwipeAsync(Guid userId)
    {
        // Undo là tính năng trả phí (Plus/Gold)
        var ent = await _subscriptionService.GetEntitlementsAsync(userId);
        if (!ent.CanUndo)
            throw new ForbiddenException("Hoàn tác (Undo) là tính năng trả phí. Nâng cấp Plus/Gold để dùng.");

        var last = await _swipeRepository.GetLastSwipeAsync(userId)
            ?? throw new BadRequestException("You have no swipe to undo.");

        // Chỉ hoàn tác được lượt Pass (bỏ qua). Lượt Like/SuperLike đã "gửi" tới đối phương
        // (có thể đã tạo match) nên không cho rút lại — giống Bumble.
        if (SwipeAction.IsLike(last.Action))
            throw new BadRequestException("Chỉ hoàn tác được lượt Bỏ qua (Pass), không hoàn tác được lượt Thích.");

        await _swipeRepository.DeleteAsync(last);
        await _swipeRepository.SaveChangesAsync();

        return new UndoResultDto
        {
            TargetUserId = last.TargetUserId,
            Action = last.Action,
            MatchRemoved = false,
        };
    }

    /// <summary>Tạo (hoặc kích hoạt lại) match cho cặp, luôn chuẩn hóa UserAId &lt; UserBId.</summary>
    private async Task<Guid> EnsureMatchAsync(Guid user1, Guid user2)
    {
        var (a, b) = user1.CompareTo(user2) < 0 ? (user1, user2) : (user2, user1);

        var existing = await _matchRepository.GetByPairAsync(a, b);
        if (existing is not null)
        {
            existing.IsActive = true; // tái match sau khi từng unmatch
            return existing.Id;
        }

        var match = new Match
        {
            Id = Guid.NewGuid(),
            UserAId = a,
            UserBId = b,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        await _matchRepository.AddAsync(match);
        return match.Id;
    }
}
