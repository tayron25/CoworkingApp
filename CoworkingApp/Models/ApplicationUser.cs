using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema; // <-- Añade esta línea

namespace CoworkingApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Propiedad para almacenar los créditos del usuario
        [Column(TypeName = "decimal(18,2)")] // Define el tipo de dato en la base de datos
        public decimal CreditosDisponibles { get; set; }
    }
}
