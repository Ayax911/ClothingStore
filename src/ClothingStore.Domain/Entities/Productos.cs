using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStore.Domain.Entities
{
    public class Productos
    {
        [Key] public int Id { get; set; }

        [MaxLength(100)]
        public string? Nombre { get; set; }

        [MaxLength(100)]
        public string? Material { get; set; }

        [MaxLength(100)]
        public string? Codigo { get; set; }

       [Column(TypeName = "decimal(18,4)")]
        public decimal ValorUnitario { get; set; }
    }

}


