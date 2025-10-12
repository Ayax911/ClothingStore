using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ClothingStore.Domain.Entities;

namespace ClothingStore.Persistence.Interfaces
{
    public interface IConexion
    {

        DbSet<Productos> Productos { get; set; }
        DbSet<Clientes> Clientes { get; set; }
        DbSet<Compras> Compras { get; set; }
        DbSet<DetallesCompras> DetallesCompras { get; set; }

        EntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();

    }
    
}


