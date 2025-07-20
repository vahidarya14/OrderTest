using Domain;

namespace Application
{
    public interface IOrderService
    {
        Task<List<Order>> ToListAsync(long? CustomerId, int skip, int take);
        Task<Factor> Create(long customerId, OrderCrreateDto dto);
    }
}