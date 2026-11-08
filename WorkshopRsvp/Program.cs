using Microsoft.EntityFrameworkCore;
using WorkshopRsvp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection.");

if (!builder.Environment.IsDevelopment() &&
    connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        "LocalDB is for local development. Configure DefaultConnection with the Azure SQL connection string before deployment.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

var app = builder.Build();

// Apply migrations automatically locally. Production can use a reviewed SQL script
// or explicitly opt in for a single-instance lab deployment.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (app.Environment.IsDevelopment() ||
        app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
    {
        await context.Database.MigrateAsync();
    }

    await DbInitializer.InitializeAsync(context);
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
