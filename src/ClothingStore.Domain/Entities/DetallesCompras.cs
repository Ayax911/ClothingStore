using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStore.Domain.Entities
{
     public class DetallesCompras
 {
    [Key]public int Id { get; set; }
    
    
     public int Cantidad { get; set; }


     [Column(TypeName = "decimal(18,4)")]
     public decimal ValorBruto { get; set; }

     //FK
     public int CompraId { get; set; }
     public int ProductoId { get; set; }


     [ForeignKey("CompraId")] public Compras? Compra { get; set; }
     [ForeignKey("ProductoId")] public Productos? Producto { get; set; }

 }
}

