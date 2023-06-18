using BL.DALModels;
using BL.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<RwaMoviesContext>(options =>
{
    options.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
});


builder.Services.AddAutoMapper(
    typeof(AdminModule.Mapping.AutomapperProfile),
    typeof(BL.Mapping.AutomapperProfile));

builder.Services.AddScoped<IMoviesRepository, VideoRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MovieController1}/{action=Index}/{id?}");

app.Run();
