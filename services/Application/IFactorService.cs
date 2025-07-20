using Domain;

namespace Application
{
    public interface IFactorService
    {
        Task<Factor> Create(int orderId, long customerId, DateTime dueDate);
        Task<bool> Pay(Guid invoiceId, long customerId);
        Task<List<FactorListdto>> ToListAsync(long? CustomerId, int skip, int take);
    }
}