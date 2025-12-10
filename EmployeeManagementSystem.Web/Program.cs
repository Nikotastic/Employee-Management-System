using EmployeeManagementSystem.Application;
using EmployeeManagementSystem.Infrastructure.DependencyInjection;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Enable legacy timestamp behavior to allow Unspecified dates in PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Environment variables are injected by Docker Compose, no need to load manually here in production/docker.
// Checks for development if needed, but removing to clean logs.

var builder = WebApplication.CreateBuilder(args);

// Configuración del sistema
builder.Configuration.AddEnvironmentVariables();

// Agregar servicios
builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar cookies para la aplicación web (NO JWT)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

// 4️⃣ Migraciones + Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EmployeeManagementSystem.Infrastructure.Data.ApplicationDbContext>();
        var userManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<EmployeeManagementSystem.Infrastructure.Identity.ApplicationUser>>();
        await EmployeeManagementSystem.Infrastructure.Data.DataSeeder.SeedAsync(context, userManager);
        Console.WriteLine("Database initialized successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error initializing databases");
    }
}

// 5️⃣ Middleware
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
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
