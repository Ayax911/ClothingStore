using ClothingStore.Application.Implementaciones;
using ClothingStore.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ClothingStore.Application
{
    public static class DependencyInjectionApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
          
            services.AddScoped<IComprasAplicacion, ComprasAplicacion>();
            services.AddScoped<IClientesAplicacion, ClientesAplicacion>();
            services.AddScoped<IDetallesComprasAplicacion, DetallesComprasAplicacion>();
            services.AddScoped<IProductosAplicacion, ProductosAplicacion>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
      

            return services;
        }
    }
}