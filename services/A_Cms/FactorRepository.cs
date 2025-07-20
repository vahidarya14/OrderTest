using Domain;
using Persistance.Data;

namespace Persistance;

public class FactorRepository(AppDbContext _db) : Repository<AppDbContext, Factor>(_db)
{
}
