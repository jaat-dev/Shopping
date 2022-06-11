using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Helpers;
using Shopping.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<User, IdentityRole>(cfg =>
{
    cfg.User.RequireUniqueEmail = true;
    cfg.Password.RequireDigit = false;
    cfg.Password.RequiredUniqueChars = 0;
    cfg.Password.RequireLowercase = false;
    cfg.Password.RequireNonAlphanumeric = false;
    cfg.Password.RequireUppercase = false;
    cfg.Password.RequiredLength = 6;
}).AddEntityFrameworkStores<DataContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/NotAuthorized";
    options.AccessDeniedPath = "/Account/NotAuthorized";
});

builder.Services.AddTransient<SeedDb>();
builder.Services.AddScoped<IUserHelper, UserHelper>();
builder.Services.AddScoped<ICombosHelper, CombosHelper>();
builder.Services.AddScoped<IBlobHelper, BlobHelper>();

var app = builder.Build();

LoadData.SeedData(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
