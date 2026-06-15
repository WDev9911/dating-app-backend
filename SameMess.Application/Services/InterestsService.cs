using SameMess.Application.DTOs.Settings;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class InterestsService : IInterestsService
{
    private const int MaxInterests = 10;

    private readonly IInterestRepository _interestRepository;
    private readonly IUserInterestRepository _userInterestRepository;

    public InterestsService(
        IInterestRepository interestRepository,
        IUserInterestRepository userInterestRepository)
    {
        _interestRepository = interestRepository;
        _userInterestRepository = userInterestRepository;
    }

    public async Task<List<InterestDto>> GetCatalogAsync()
    {
        var items = await _interestRepository.GetActiveAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<List<InterestDto>> GetMyInterestsAsync(Guid userId)
    {
        var items = await _userInterestRepository.GetByUserIdAsync(userId);
        return items
            .Select(ui => ToDto(ui.Interest))
            .OrderBy(i => i.GroupName).ThenBy(i => i.Name)
            .ToList();
    }

    public async Task<List<InterestDto>> UpdateMyInterestsAsync(Guid userId, UpdateInterestsDto dto)
    {
        var ids = dto.InterestIds.Distinct().ToList();

        if (ids.Count > MaxInterests)
            throw new BadRequestException($"Chỉ chọn được tối đa {MaxInterests} sở thích.");

        if (ids.Count > 0)
        {
            var valid = await _interestRepository.GetByIdsAsync(ids);
            if (valid.Count != ids.Count || valid.Any(i => !i.IsActive))
                throw new BadRequestException("Danh sách sở thích chứa mục không hợp lệ.");
        }

        await _userInterestRepository.ReplaceForUserAsync(userId, ids);
        await _userInterestRepository.SaveChangesAsync();

        return await GetMyInterestsAsync(userId);
    }

    private static InterestDto ToDto(Interest i) =>
        new() { Id = i.Id, Name = i.Name, GroupName = i.GroupName };
}
