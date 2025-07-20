using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Lib;

public static class ExtClaimsPrincipal
{

    public static string Email(this ClaimsPrincipal user)
        => user.Identity.IsAuthenticated ? user.Claims.First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value : "";

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        var roles = user.Identity.IsAuthenticated ? user.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == "Admin") : false;


        return roles;
    }

    public static long GetUserId(this ClaimsPrincipal user)
       => long.Parse(user.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);

    public static string UserName(this ClaimsPrincipal user)
        => user.Claims.First(x => x.Type == "username").Value;
}
