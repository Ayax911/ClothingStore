using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ClothingStore.Application;
using ClothingStore.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ========== CONFIGURACIÓN DE JWT ==========
var jwtKey = builder.Configuration["JwtSettings:Key"] 
    ?? throw new InvalidOperationException("JWT Key no configurada en appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Cambiar a true en producción
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ========== CONFIGURACIÓN DE CORS (opcional) ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ========== INYECCIÓN DE DEPENDENCIAS ==========
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

// ========== CONTROLADORES Y SWAGGER ==========
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "ClothingStore API", 
        Version = "v1",
        Description = "API para gestión de tienda de ropa con autenticación JWT"
    });

    // 🔐 Configuración de seguridad JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Autenticación JWT usando el esquema Bearer. 
                      
Ingresa 'Bearer' seguido de un espacio y luego tu token.

Ejemplo: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ========== CONSTRUCCIÓN DE LA APP ==========
var app = builder.Build();

// ========== MIDDLEWARE ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClothingStore API V1");
    });
}

app.UseHttpsRedirection();

// 🔒 IMPORTANTE: El orden de estos middlewares es crucial
app.UseCors("AllowAll");      // 1. CORS (si lo necesitas)
app.UseAuthentication();       // 2. Autenticación JWT
app.UseAuthorization();        // 3. Autorización

app.MapControllers();

app.Run();