using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class NudgeDismissalRepository : BaseRepository<NudgeDismissal>, INudgeDismissalRepository
{
    public NudgeDismissalRepository(AppDbContext context) : base(context) { }

    public async Task<List<string>> GetCodesAsync(Guid userId, Guid conversationId) =>
        await _dbSet.Where(d => d.UserId == userId && d.ConversationId == conversationId)
            .Select(d => d.NudgeCode)
            .ToListAsync();
}

public class MeetupProposalRepository : BaseRepository<MeetupProposal>, IMeetupProposalRepository
{
    public MeetupProposalRepository(AppDbContext context) : base(context) { }
}
