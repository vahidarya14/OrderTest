using Application;
using Application.Lib;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Persistance.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class ProgramEndpoints
{
    public static void UseApplicationEndPoints(this WebApplication app)
    {

        app.MapGet("/api/profile", [Authorize] async ([FromServices] IUserService userService, ClaimsPrincipal claimsPrincipal)
            => await userService.Profile(claimsPrincipal.UserName()));


        app.MapGet("/api/users", [Authorize(Roles = "Admin")] async ([FromServices] IUserService userService)
            => await userService.ToListAsync(0, 10));


        app.MapPost("/api/order", [Authorize] async ([FromServices] IOrderService orderService, ClaimsPrincipal claimsPrincipal, OrderCrreateDto dto)
         => await orderService.Create(claimsPrincipal.GetUserId(), dto));


        app.MapGet("/api/orders", [Authorize] async ([FromServices] IOrderService orderService, ClaimsPrincipal claimsPrincipal) =>
        {
            var isAdmin = claimsPrincipal.IsAdmin();
            return await orderService.ToListAsync(isAdmin ? null : claimsPrincipal.GetUserId(), 0, 20);
        });


        app.MapGet("/api/factors", [Authorize] async ([FromServices] IFactorService factorService, ClaimsPrincipal claimsPrincipal) =>
        {
            var isAdmin = claimsPrincipal.IsAdmin();
            return await factorService.ToListAsync(isAdmin ? null : claimsPrincipal.GetUserId(), 0, 20);
        });


        app.MapPost("/api/factor", [Authorize(Roles = "Admin")] async ([FromServices] IFactorService factorService, int orderId, long customerId, DateTime dueDate)
            => factorService.Create(orderId, customerId, dueDate));


        app.MapPost("/api/factor/{invoiceId}/pay", [Authorize] async ([FromServices] IFactorService factorService, ClaimsPrincipal claimsPrincipal, Guid invoiceId)
            => factorService.Pay(invoiceId, claimsPrincipal.GetUserId()));
    }


    public static void UseLoginEndPoints(this WebApplication app)
    {
        app.MapPost("/api/login", async (IConfiguration Configuration,
                                         UserManager<AppUser> userManager,
                                         SignInManager<AppUser> signInManager,
                                         RoleManager<AppRole> roleManager,
                                         string userName, string password) =>
        {
            try
            {
                if (await signInManager.PasswordSignInAsync(userName, password, isPersistent: false, lockoutOnFailure: false) == Microsoft.AspNetCore.Identity.SignInResult.Failed)
                    throw new Exception("رمز یا نام کاربری اشتباه");

                var applicationUser = await userManager.FindByNameAsync(userName);
                var roles = await userManager.GetRolesAsync(applicationUser);

                string token = GenerateJSONWebToken(applicationUser, roles);
                return token;
            }
            catch (Exception ex)
            {
                throw;
            }


            string GenerateJSONWebToken(AppUser userInfo, IList<string> roles)
            {
                var key = Configuration["JWT:Secret"];
                var Issuer = Configuration["JWT:ValidIssuer"];

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                List<Claim> claims = new() {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim("username", userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
                foreach (var item in roles)
                {
                    //claims.Add(new Claim("Role", item));
                    claims.Add(new Claim(ClaimTypes.Role, item));
                }

                var token = new JwtSecurityToken(Issuer,
                    Issuer,
                    claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        });
    }


    public static void UseSeedDataEndPoints(this WebApplication app)
    {
        app.MapGet("/seed-data", async (SeedData db) => await db.Seed());
    }
}