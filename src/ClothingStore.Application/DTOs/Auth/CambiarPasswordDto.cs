using System;
using System.ComponentModel.DataAnnotations;

namespace ClothingStore.Application.DTOs
{
    public class CambiarPasswordDto
{
   
    [Required]
    public string? PasswordActual { get; set; }

  
    [MinLength(6)]
    public string? NuevaPassword { get; set; }

    
    [Compare("NuevaPassword")]
    public string? ConfirmarPassword { get; set; }
}
}

