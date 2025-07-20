using Domain;
using Persistance.Data;

namespace Persistance;

public class OrderRepository(AppDbContext _db) : Repository<AppDbContext, Order>(_db)
{
}
