using Microsoft.AspNetCore.Identity;
using RedMaskFramework.DDD;

namespace Domain;

public class Customer : IdentityUser<long>, IAggregateRoot//customer
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}


public class AppRole : IdentityRole<long>
{
}