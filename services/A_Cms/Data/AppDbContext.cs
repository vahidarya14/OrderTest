using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Data;

//add-migration init -p A_Common -s OrderApi -context AppDbContext
//update-database -context AppDbContext
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, AppRole, long>(options)
{
    public DbSet<Factor> Factors { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        builder.Entity<Wallet>(x =>
        {
            x.OwnsOne(x=>x.Walletsetting);
        });


    }
}