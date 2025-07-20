using Domain;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System.Security.Claims;

namespace Application;

public class UserService(AppDbContext db) : IUserService
{
    public AppUser Profile(string userName)
    {
        var user = db.Users.FirstOrDefault(x => x.UserName == userName);
        return user;
    }

    public async Task<List<AppUser>> ToListAsync(int skip, int take)
    {
        var user = await db.Users.AsNoTracking().ToListAsync();
        return user;
    }
}
