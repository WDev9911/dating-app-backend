using Microsoft.AspNetCore.SignalR;

namespace SameMess.API.Services;

/// <summary>
/// SignalR mặc định định danh user bằng ClaimTypes.NameIdentifier. Vì JWT của app dùng claim "sub"
/// (và MapInboundClaims = false), cần provider này để Clients.User(...) hoạt động đúng.
/// </summary>
public class SubClaimUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirst("sub")?.Value;
}
