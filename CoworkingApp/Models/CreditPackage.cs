using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoworkingApp.Models
{
    public class CreditPackage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del paquete es obligatorio.")]
        [Display(Name = "Nombre del Paquete")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio (en Bs)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La cantidad de créditos es obligatoria.")]
        [Display(Name = "Créditos que otorga")]
        public int Credits { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
    }
}