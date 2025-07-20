using Domain;

namespace Application
{
    public interface IUserService
    {
        Task<List<AppUser>> ToListAsync(int skip, int take);
        Task<AppUser> Profile(string userName);
    }
}