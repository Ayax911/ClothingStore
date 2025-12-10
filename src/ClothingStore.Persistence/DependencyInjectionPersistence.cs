using ClothingStore.Persistence.Implementaciones;
using ClothingStore.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClothingStore.Persistence
{
    public static class DependencyInjectionPersistence
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            
            services.AddDbContext<Conexion>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.MigrationsAssembly("ClothingStore.Persistence")
                );
            });

           
            services.AddScoped<IConexion, Conexion>();

            return services;
        }
    }
}