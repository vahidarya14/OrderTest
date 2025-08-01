using Application;
using Application.Lib;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistance;
using Persistance.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services
     .AddIdentity<Customer, AppRole>(o =>
     {
         o.Password.RequireLowercase = false;
         o.Password.RequireNonAlphanumeric = false;
         o.Password.RequireDigit = false;
         o.Password.RequireUppercase = false;
         o.Stores.MaxLengthForKeys = 128;
         o.SignIn.RequireConfirmedAccount = false;
     })
     .AddEntityFrameworkStores<AppDbContext>();

//builder.Services.ConfigureApplicationCookie(o =>
//{
//    o.ExpireTimeSpan = TimeSpan.FromDays(1);
//    o.Cookie.MaxAge = TimeSpan.FromDays(1);
//    o.SlidingExpiration = false;
//    //o.Cookie.Name = "RM.CMS._ah";
//    o.Cookie.HttpOnly = true;
//    ////o.AccessDeniedPath = "/Identity/Account/AccessDenied";
//    //o.LoginPath = "/Identity/Account/Login";
//    //o.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter; // requires using Microsoft.AspNetCore.Authentication.Cookies;
//});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });
builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("sql") ?? throw new InvalidOperationException("Connection string 'sql' not found.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<SeedData>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/seed-data", async (SeedData db) => await db.Seed());

app.MapGet("/api/profile", [Authorize] async ([FromServices] IUserService userService, ClaimsPrincipal claimsPrincipal)
            => await userService.Profile(claimsPrincipal.UserName()));


app.MapGet("/api/users", [Authorize(Roles = "Admin")] async ([FromServices] IUserService userService)
    => await userService.ToListAsync(0, 10));


app.MapPost("/api/login", async (IConfiguration Configuration,
                                 UserManager<Customer> userManager,
                                 SignInManager<Customer> signInManager,
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


    string GenerateJSONWebToken(Customer userInfo, IList<string> roles)
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


app.Run();
