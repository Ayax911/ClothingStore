using Microsoft.EntityFrameworkCore;
using ClothingStore.Domain.Entities;
using ClothingStore.Persistence.Interfaces;


namespace ClothingStore.Persistence.Implementaciones
{
    public class Conexion : DbContext, IConexion
    {
        public Conexion(DbContextOptions<Conexion> options) : base(options)
        {
            // El string de conexión se inyecta a través de DbContextOptions
        }

        public DbSet<Productos> Productos { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Compras> Compras { get; set; }
        public DbSet<DetallesCompras> DetallesCompras { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones de tus entidades aquí
            base.OnModelCreating(modelBuilder);
        }

        



    }
}


