using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task RevokeAllUserTokensAsync(Guid userId);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId);
}
