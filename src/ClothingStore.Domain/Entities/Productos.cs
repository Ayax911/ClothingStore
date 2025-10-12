using System.ComponentModel.DataAnnotations;

namespace ClothingStore.Domain.Entities
{
    public class Productos
    {
        [Key] public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Material { get; set; }
        public string? Codigo { get; set; }
        public decimal ValorUnitario { get; set; }
    }

}


