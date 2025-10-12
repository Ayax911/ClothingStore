using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStore.Domain.Entities
{
     public class DetallesCompras
 {
     [Key]  public int Id { get; set; }
     public int Cantidad { get; set; }
     public decimal ValorBruto { get; set; }

     //FK
     public int Compra { get; set; }
     public int Producto { get; set; }


     [ForeignKey("Compra")] public Compras? _Compra { get; set; }
     [ForeignKey("Producto")] public Productos? _Producto { get; set; }

 }
}

