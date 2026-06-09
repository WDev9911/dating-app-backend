using SameMess.Domain.Entities;

namespace SameMess.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    (string PlainToken, string TokenHash) GenerateRefreshToken();
    string HashToken(string token);
}
