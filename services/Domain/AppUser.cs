using Microsoft.AspNetCore.Identity;
using RedMaskFramework.DDD;

namespace Domain;

public class AppUser : IdentityUser<long>, IAggregateRoot//customer
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Avatar { get; set; }
    public long WalletBalance { get; set; }


    public static AppUser Empty() => new AppUser();

}


public class AppRole : IdentityRole<long>
{
}