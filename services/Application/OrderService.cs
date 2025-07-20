using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Persistance.Data;
using System.Security.Claims;

namespace Application;

public class OrderService(AppDbContext db) : IOrderService
{

    public async Task<List<Order>> ToListAsync(long? CustomerId, int skip, int take)
    {
        var q = db.Orders.AsQueryable();

        if (CustomerId.HasValue)
            q = q.Where(x => x.CustomerId == CustomerId);

        return await q.AsNoTracking().OrderByDescending(x => x.OrderDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
