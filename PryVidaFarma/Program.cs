using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PryVidaFarma.DAO;
using PryVidaFarma.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar DAO como servicios Scoped
builder.Services.AddScoped<ProductosDAO>();
builder.Services.AddScoped<CategoriasDAO>();
builder.Services.AddScoped<CarritoDao>();

// Configurar DbContext con SQL Server
builder.Services.AddDbContext<AplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cad_cn")));

// Configurar servicios de sesi�n
builder.Services.AddDistributedMemoryCache(); // Necesario para almacenar las sesiones en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Duraci�n de la sesi�n
    options.Cookie.HttpOnly = true; // Aumenta la seguridad al evitar acceso de JavaScript
    options.Cookie.IsEssential = true; // Para cumplir con RGPD/GDPR
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
});

// Configurar controladores y vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware para manejar excepciones
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

// Middleware para enrutamiento
app.UseRouting();

// Middleware para servir archivos est�ticos
app.UseStaticFiles();

// Middleware de sesi�n
app.UseSession(); // Este middleware debe estar presente y configurado

// Middleware de autenticaci�n y autorizaci�n (Eliminar si no usas autenticaci�n con cookies)
// app.UseAuthentication(); 
app.UseAuthorization();

// Configurar rutas predeterminadas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
