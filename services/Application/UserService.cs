using Domain;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace Application;

public class UserService(UserRepository db) : IUserService
{
    public async Task<AppUser> Profile(string userName)
    {
        var user =await db.FirstOrDefaultAsync(x => x.UserName == userName);
        return user;
    }

    public async Task<List<AppUser>> ToListAsync(int skip, int take)
    {
        var user = await db.AsNoTracking().ToListAsync();
        return user;
    }
}
public interface IUserService
{
    Task<List<AppUser>> ToListAsync(int skip, int take);
    Task<AppUser> Profile(string userName);
}
