using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IAppSettingRepository
{
    Task<List<AppSetting>> GetAllAsync();
    Task UpsertAsync(string key, string value);
    Task<int> SaveChangesAsync();
}
