using AnimalCare.Data;
using AnimalCare.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2) Register DbContext with SQL Server
builder.Services.AddDbContext<AnimalCareDbContext>(options =>
    options.UseSqlServer(connectionString));

// (Optional but useful in dev for better EF error pages)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3) Register Identity with ApplicationUser + Roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // dev only
                                                    // You can also configure password rules here
})
    .AddRoles<IdentityRole>() // we use roles: Admin, Veterinarian, Receptionist
    .AddEntityFrameworkStores<AnimalCareDbContext>();

// 4) Add MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5) Configure middleware pipeline

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // shows migration errors nicely
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Identity
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity endpoints (login, register, etc.)
app.MapRazorPages();

app.Run();
