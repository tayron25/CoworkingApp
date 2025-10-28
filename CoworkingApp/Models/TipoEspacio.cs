using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoworkingApp.Models
{
    public class TipoEspacio
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre del Espacio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El costo en créditos es obligatorio")]
        [Display(Name = "Costo (Créditos por Hora)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoCreditosHora { get; set; }
    }
}