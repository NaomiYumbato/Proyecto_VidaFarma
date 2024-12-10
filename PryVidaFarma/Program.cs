using PryVidaFarma.DAO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1.- Agregar el DAO EcommerceDAO para ser utilizado
builder.Services.AddScoped<CarritoDao>();

// 2.- Establecer el tiempo de la duración de las variables de Session
builder.Services.AddSession(
    s => s.IdleTimeout = TimeSpan.FromMinutes(20));

var app = builder.Build();

//3 .- Habilitar las variables de Sesion
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
