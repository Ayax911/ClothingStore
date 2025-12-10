using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStore.Domain.Entities
{
    public class Compras
  {
      
      [Key] public int Id { get; set; }

      public DateTime Fecha { get; set; }

      [MaxLength(100)]
      public string? Codigo { get; set; }
      
     [Column(TypeName = "decimal(18,4)")]
      public decimal ValorTotal { get; set; }

      //FK
      public int ClienteId { get; set; }

     [ForeignKey("ClienteId")] public Clientes? Cliente { get; set; }
     
      public List<DetallesCompras>? DetallesCompras { get; set; }

    }
}



