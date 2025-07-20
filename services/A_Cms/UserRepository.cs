using Domain;
using Persistance.Data;

namespace Persistance;

public class UserRepository(AppDbContext _db) : Repository<AppDbContext, AppUser>(_db)
{
}
