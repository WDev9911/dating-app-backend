using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class SafetyCheckInRepository : BaseRepository<SafetyCheckIn>, ISafetyCheckInRepository
{
    public SafetyCheckInRepository(AppDbContext context) : base(context) { }
}
