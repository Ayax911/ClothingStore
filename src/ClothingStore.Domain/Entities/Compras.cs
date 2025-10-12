using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStore.Domain.Entities
{
    public class Compras
    {
      [Key] public int Id { get; set; }
      public DateTime Fecha { get; set; }
      public string? Codigo { get; set; }
      public decimal ValorTotal { get; set; }

      //FK
      public int Cliente { get; set; }
     
      [ForeignKey("Cliente")] public Clientes? _Cliente { get; set; }
     
      public List<DetallesCompras>? DetallesCompras { get; set; }

    }
}



