using Application;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistance;
using Persistance.Data;
using RedMaskFramework;
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
     .AddIdentity<AppUser, AppRole>(o =>
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
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddScoped<IFactorService,FactorService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<FactorRepository>();
builder.Services.AddScoped<OrderRepository>();

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

app.UseSeedDataEndPoints();
app.UseLoginEndPoints();
app.UseApplicationEndPoints();

app.Run();
