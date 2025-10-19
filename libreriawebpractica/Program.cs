using Microsoft.EntityFrameworkCore; // 1. Agregar esta línea para usar EF Core
using libreriawebpractica.Data; // 2. Agregar esta línea para referenciar tu DbContext en la carpeta Data

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();

// 3. Agregar DbContext CON EL NOMBRE CONCRETO: AppDbContext
builder.Services.AddDbContext<AppDbContext>(options => // Cambiado de DbContext a AppDbContext
    options.UseSqlServer( // Usa UseSqlServer para SQL Server
        builder.Configuration.GetConnectionString("DefaultConnection") // Lee la cadena de conexión
    ));

var app = builder.Build();

// Configurar el pipeline de HTTP request.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // El valor predeterminado de HSTS es 30 días. Puedes cambiarlo para escenarios de producción, consulta https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Importante para servir archivos estáticos como imágenes, CSS, JS

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();