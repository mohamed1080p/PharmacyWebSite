using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PharmacyWebSite.Data;
using PharmacyWebSite.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        // Apply pending migrations
        db.Database.Migrate();

        // Seed medicines
        if (!db.Medicines.Any())
        {
            db.Medicines.AddRange(
                new Medicine
                {
                    Name = "Panadol",
                    Price = 15.99m,
                    Stock = 100,
                    Category = "Pain Relief",
                    Description = "Effective pain relief medication",
                    ImagePath = "/images/default-painkiller.jpg"
                },
                new Medicine
                {
                    Name = "Vitamin C",
                    Price = 29.99m,
                    Stock = 50,
                    Category = "Supplements",
                    Description = "Immune system booster",
                    ImagePath = "/images/default-vitamin.jpg"
                }
            );
        }

        // Seed admin user
        if (!db.Users.Any(u => u.IsAdmin))
        {
            db.Users.Add(new User
            {
                Name = "Admin",
                Email = "admin@pharmacy.com",
                Password = "Admin@123",
                PhoneNumber = "+1234567890",
                IsAdmin = true
            });
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database seeding failed: {ex.Message}");
    }
}

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();