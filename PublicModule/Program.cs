using BL.DALModels;
using BL.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<RwaMoviesContext>(options => {
    options.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
});

builder.Services.AddAutoMapper(
    typeof(PublicModule.Mapping.AutomapperProfile),
    typeof(BL.Mapping.AutomapperProfile));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMoviesRepository, VideoRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Register}/{id?}");

app.Run();
