using FurnitureProject.Data;
using FurnitureProject.Middleware;
using FurnitureProject.Models;
using FurnitureProject.Repositories;
using FurnitureProject.Services;
using FurnitureProject.Services.Email;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Kết nối database posgresql
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IPromotionService, PromotionService>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
 {
     option.IdleTimeout = TimeSpan.FromMinutes(30);
     option.Cookie.HttpOnly = true;
     option.Cookie.IsEssential = true;
 });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.MapGet("/test-db", async (AppDbContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        return canConnect ? "✅ Connect successfully" : "❌ Connect failed";
    }
    catch (Exception ex)
    {
        return $"❌ Error: {ex.Message}";
    }
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Auto create database data if not exist
    await DataSeeder.SeedAsync(db);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseMiddleware<RoleAuthorizationMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
