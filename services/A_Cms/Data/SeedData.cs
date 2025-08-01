using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistance.Data;
public class Consts
{
    public static string Admin => "Admin";
}
public class SeedData(AppDbContext db, UserManager<Customer> userMgmt, RoleManager<AppRole> roleMgmt, IConfiguration configuration)
{
    public async Task Seed()
    {
        if (!await roleMgmt.RoleExistsAsync(Consts.Admin))
            await roleMgmt.CreateAsync(new AppRole { Name = Consts.Admin});

        if (!await db.Users.AnyAsync())
        {
            var admin = new Customer
            {
                FirstName = "وحید",
                LastName = "آریا",
                UserName = "admin",
                Email = "a2@gmail.com"
            };
            await userMgmt.CreateAsync(admin, "123456");
            await userMgmt.AddToRoleAsync(admin, Consts.Admin);


            var user2 = new Customer
            {
                FirstName = "test",
                LastName = "test",
                UserName = "09192962584",
                Email = "user1",
            };
            await userMgmt.CreateAsync(user2, "123456");

            await db.SaveChangesAsync();

            var wallet = new Wallet()
            {
                UserId = user2.Id,
                Setting = new WalletSetting
                {
                    MinBalanceWithdrawlThreshold = -150
                },
                FreeBalance = 10000000000
            };
            await db.Wallets.AddAsync(wallet);
            await db.SaveChangesAsync();
        }


        var aa = await db.SaveChangesAsync();
    }
}
