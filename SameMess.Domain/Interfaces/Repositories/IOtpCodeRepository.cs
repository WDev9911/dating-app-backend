using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IOtpCodeRepository : IBaseRepository<OtpCode>
{
    Task<OtpCode?> GetValidOtpAsync(string email, string code, string purpose);
    Task InvalidateAllOtpsForEmailAsync(string email, string purpose);
}
