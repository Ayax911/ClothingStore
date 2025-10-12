using Microsoft.OpenApi.Models;
using ClothingStore.Application;   // <- Importa tus extensiones
using ClothingStore.Persistence;  // <- Importa tus extensiones

var builder = WebApplication.CreateBuilder(args);

// 👇 Agrega tus módulos centralizados de inyección
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

// Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ClothingStore API", Version = "v1" });
});

// Construir app
var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseAuthentication(); // 🔒 cuando implementes JWT
app.UseAuthorization();

app.MapControllers();
app.Run();
