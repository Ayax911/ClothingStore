using Microsoft.EntityFrameworkCore;
using ClothingStore.Persistence.Implementaciones;
using ClothingStore.Persistence.Interfaces;

var builder = WebApplication.CreateBuilder(args);
//Initialize the Api
// Add services to the container.

builder.Services.AddDbContext<Conexion>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly("ClothingStore.Persistence")
    );
});

builder.Services.AddScoped<IConexion, Conexion>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();//Inyecta servicios
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseAuthorization(); //Crear modelo de seguridad

app.MapControllers();

app.Run();
