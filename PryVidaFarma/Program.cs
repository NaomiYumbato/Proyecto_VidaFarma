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

// Configurar servicios de sesión
builder.Services.AddDistributedMemoryCache(); // Necesario para almacenar las sesiones en memoria
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Duración de la sesión
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

// Middleware para servir archivos estáticos
app.UseStaticFiles();

// Middleware de sesión
app.UseSession(); // Este middleware debe estar presente y configurado

// Middleware de autenticación y autorización (Eliminar si no usas autenticación con cookies)
// app.UseAuthentication(); 
app.UseAuthorization();

// Configurar rutas predeterminadas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
