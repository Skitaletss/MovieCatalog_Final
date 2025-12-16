using Microsoft.EntityFrameworkCore;
using MovieCatalog.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        context.Database.EnsureCreated();

        Console.WriteLine("База даних успішно створена або вже існує!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("ПОМИЛКА при створенні бази даних!");
        Console.WriteLine($"Деталі: {ex.Message}");
        Console.WriteLine($"Повна помилка: {ex}");

        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine("Застосунок запущено!");
Console.WriteLine("Відкрийте: https://localhost:5001");

app.Run();