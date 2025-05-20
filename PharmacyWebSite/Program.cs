using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PharmacyWebSite.Data;
using PharmacyWebSite.Models;
using PharmacyWebSite.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Register EmailSender as Singleton
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Register configuration service as Singleton
builder.Services.AddSingleton<AppConfigService>();

// Authentication setup
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

// Database seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        db.Database.Migrate();

        if (!db.Medicines.Any())
        {
            var medicines = new List<Medicine>
            {
                new Medicine { Name = "Panadol Extra", Description = "مسكن للآلام وخافض للحرارة", Price = 25, Category = "Pain Relief", Stock = 50, ImagePath = "~/Images/panadol.jpg" },
                new Medicine { Name = "Antinal", Description = "لعلاج الإسهال", Price = 15, Category = "Gastrointestinal", Stock = 30, ImagePath = "~/Images/antinal.jpg" },
                new Medicine { Name = "Cataflam", Description = "مسكن للآلام ومضاد للالتهاب", Price = 45, Category = "Pain Relief", Stock = 20, ImagePath = "~/Images/cataflam.jpg" },
                new Medicine { Name = "Pantoloc", Description = "يخفف من احماض المعدة", Price = 55, Category = "proton-pump inhibitors", Stock = 25, ImagePath = "~/Images/pantoloc.jpg" },
                new Medicine { Name = "Brufen", Description = "مسكن وخافض للحرارة", Price = 40, Category = "Pain Relief", Stock = 15, ImagePath = "~/Images/brufen.jpg" },
                new Medicine { Name = "Flagyl", Description = "مضاد للطفيليات والبكتيريا", Price = 20, Category = "Antibiotics", Stock = 10, ImagePath = "~/Images/flagyl.jpg" },
                new Medicine { Name = "Augmentin", Description = "مضاد حيوي قوي", Price = 60, Category = "Antibiotics", Stock = 20, ImagePath = "~/Images/augmentin.jpg" },
                new Medicine { Name = "Ventolin", Description = "موسع للشعب الهوائية", Price = 30, Category = "Respiratory", Stock = 15, ImagePath = "~/Images/ventolin.jpg" },
                new Medicine { Name = "Concor", Description = "لعلاج ضغط الدم", Price = 70, Category = "Cardiovascular", Stock = 10, ImagePath = "~/Images/concor.jpg" },
                new Medicine { Name = "Neuroton", Description = "مقوي للأعصاب", Price = 35, Category = "Supplements", Stock = 25, ImagePath = "~/Images/neuroton.jpg" },

            };
            db.Medicines.AddRange(medicines);
            db.SaveChanges();
        }

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

// Configure the HTTP request pipeline.
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