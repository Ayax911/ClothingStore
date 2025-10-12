using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStore.Domain.Entities
{
    public class Clientes
    {
        [Key] public int Id { get; set; }
       
       [Required(ErrorMessage = "La cédula es obligatoria")]
        public string? Cedula { get; set; }

        [MaxLength(100)]
        public string? Nombre { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }
        
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; } 
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow; 

        public Usuarios? Usuario { get; set; }

        public List<Compras>? Compras { get; set; }
    }
}


