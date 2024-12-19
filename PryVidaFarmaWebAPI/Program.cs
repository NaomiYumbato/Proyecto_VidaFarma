using Microsoft.EntityFrameworkCore;
using PryVidaFarmaWebAPI.Models;

var builder = WebApplication.CreateBuilder(args);

var cadcn = builder.Configuration.GetConnectionString("cn");

builder.Services.AddDbContext<BdFarmaciaContext>(
    opt => opt.UseSqlServer(cadcn));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
