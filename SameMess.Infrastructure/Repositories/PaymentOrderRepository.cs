using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class PaymentOrderRepository : BaseRepository<PaymentOrder>, IPaymentOrderRepository
{
    public PaymentOrderRepository(AppDbContext context) : base(context) { }

    public async Task<PaymentOrder?> GetByTxnRefAsync(string txnRef) =>
        await _dbSet.FirstOrDefaultAsync(o => o.TxnRef == txnRef);
}
