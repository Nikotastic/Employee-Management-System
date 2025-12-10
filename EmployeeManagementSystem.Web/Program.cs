using EmployeeManagementSystem.Application;
using EmployeeManagementSystem.Infrastructure.DependencyInjection;
using DotNetEnv;

// Cargar variables de entorno desde el archivo .env
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($"✅ Variables de entorno cargadas desde: {envPath}");
}
else
{
    Console.WriteLine($"⚠️ Archivo .env no encontrado en: {envPath}");
}

var builder = WebApplication.CreateBuilder(args);

// Agregar variables de entorno a la configuración
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Aplicar migraciones y seed data
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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