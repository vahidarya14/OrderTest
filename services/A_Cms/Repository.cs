using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RedMaskFramework;
using RedMaskFramework.DDD;
using System.Linq.Expressions;

namespace Persistance;

public class Repository<TDbContext, T>(TDbContext db) : IRepository<T>
                                            where TDbContext : DbContext, IUnitOfWork
                                            where T : class, IAggregateRoot
{
    protected TDbContext _db = db;
    public IUnitOfWork UnitOfWork => _db;

    public IIncludableQueryable<T, TProp> Include<TProp>(Expression<Func<T, TProp>> navigationPropertyPath)
    {
        var q = _db.Set<T>().Include(navigationPropertyPath);
        return q;
    }

    public async Task<T> UpdateAsync(T item, CancellationToken ct = default)
    {
        _db.Update(item);
        return item;
    }

    public async Task<T> AddAsync(T item, CancellationToken ct = default)
    {
        await _db.Set<T>().AddAsync(item).ConfigureAwait(false);
        return item;
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _db.Set<T>().AnyAsync(predicate, ct).ConfigureAwait(false);

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _db.Set<T>().FirstOrDefaultAsync(predicate, ct).ConfigureAwait(false);

    public T Remove(T c)
    {
        _db.Set<T>().Remove(c);
        return c;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
        => await UnitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);

    public async Task<int> CountAsync(CancellationToken ct = default)
        => await _db.Set<T>().CountAsync(ct).ConfigureAwait(false);

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    => await _db.Set<T>().CountAsync(predicate, ct).ConfigureAwait(false);

    public IQueryable<TModel> Select<TModel>(Expression<Func<T, TModel>> selector)
    => _db.Set<T>().Select(selector);

    public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
     => _db.Set<T>().Where(predicate);

    public IQueryable<T> AsQueryable() => _db.Set<T>().AsQueryable();

    public IQueryable<T> AsNoTracking() => _db.Set<T>().AsNoTracking();

    public async Task<TDto?> FirstOrDefaultAsync<TDto>(Expression<Func<T, bool>> predicate, Func<T, TDto> convert, CancellationToken ct = default)
    {
        var f = await _db.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate, ct).ConfigureAwait(false);
        if (f == null) return default;

        return convert(f);
    }

}