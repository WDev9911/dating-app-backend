using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IFeatureFlagRepository
{
    Task<List<FeatureFlag>> GetAllAsync();
    Task UpsertAsync(string key, bool isEnabled);
    Task<int> SaveChangesAsync();
}
