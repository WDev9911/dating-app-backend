using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IPaymentOrderRepository : IBaseRepository<PaymentOrder>
{
    Task<PaymentOrder?> GetByTxnRefAsync(string txnRef);
}
