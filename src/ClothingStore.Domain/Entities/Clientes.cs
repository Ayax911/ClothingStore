using System.ComponentModel.DataAnnotations;

namespace ClothingStore.Domain.Entities
{
    public class Clientes
    {
        [Key] public int Id { get; set; }
       
       [Required(ErrorMessage = "La cédula es obligatoria")]
        public string? Cedula { get; set; }

        [MaxLength(100)]
        public string? Nombre { get; set; }

        [MaxLength(100)]
        public string? Telefono { get; set; }

        public List<Compras>? Compras { get; set; }
    }
}


