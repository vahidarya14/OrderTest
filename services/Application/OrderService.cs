using Domain;
using Microsoft.EntityFrameworkCore;
using Persistance;
using RedMaskFramework.DDD;

namespace Application;

public class OrderService(OrderRepository orderRepo) : IOrderService
{
    public async Task<Order> Create(long customerId,OrderCrreateDto dto)
    {
        var newOrder = new Order
        {
            CustomerId = customerId,
            TotalAmount = dto.TotalAmount,
            OrderDate = DateTime.UtcNow,
            Product=dto.Product,
            Quantity= dto.Quantity
        };
        await orderRepo.AddAsync(newOrder);
        await orderRepo.SaveChangesAsync(CancellationToken.None);
        return newOrder;
    }

    public async Task<List<Order>> ToListAsync(long? CustomerId, int skip, int take)
    {
        var q = orderRepo.AsQueryable();

        if (CustomerId.HasValue)
            q = q.Where(x => x.CustomerId == CustomerId);

        return await q.AsNoTracking().OrderByDescending(x => x.OrderDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}

public record OrderCrreateDto
{
    public string Product { get; set; }
    public int Quantity { get; set; }
    public long TotalAmount { get; set; }
}