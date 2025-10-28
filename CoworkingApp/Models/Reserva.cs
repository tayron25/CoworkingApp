using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingApp.Models
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        
        public string? UsuarioId { get; set; } // FK a la tabla de usuarios de Identity

        [Required]
        public int TipoEspacioId { get; set; } // FK a la tabla de TipoEspacio

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Inicio de la Reserva")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [Display(Name = "Fin de la Reserva")]
        public DateTime FechaFin { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser? Usuario { get; set; }

        [ForeignKey("TipoEspacioId")]
        public virtual TipoEspacio? TipoEspacio { get; set; }
    }
}