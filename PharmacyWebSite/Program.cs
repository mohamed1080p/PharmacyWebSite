using Microsoft.EntityFrameworkCore;
using PharmacyWebSite.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

var app = builder.Build();
// Inside the using scope in Program.cs
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Add admin user if none exists
    if (!db.Users.Any(u => u.IsAdmin))
    {
        var admin = new User
        {
            Name = "Admin",
            Email = "admin@dawaya.com",
            Password = "admin@123", // Temporary password
            PhoneNumber = "+123456789",
            IsAdmin = true
        };
        db.Users.Add(admin);
        db.SaveChanges();
    }
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();