using Microsoft.EntityFrameworkCore;
using PryVidaFarma.DAO;
using PryVidaFarma.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ProductosDAO>();
builder.Services.AddScoped<CategoriasDAO>();

builder.Services.AddDbContext<AplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cad_cn")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
