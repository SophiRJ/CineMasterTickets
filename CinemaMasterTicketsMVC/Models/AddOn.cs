using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaMasterTicketsMVC.Models
{
    public enum AddOnType { Comida, Bebida, Otros }

    public class AddOn
    {
        public int AddOnId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "El nombre debe contener entre 4 y 50 caracteres")]
        public string? AddOnName { get; set; }

        public string? AddOnImage { get; set; }

        [Required(ErrorMessage = "Debe indicar el precio")]
        [Range(0.01, 1000, ErrorMessage = "El precio debe ser mayor que 0 y menor que 1000")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        // no persistida para la imagen que sube el admin
        [NotMapped]
        [Display(Name = "Imagen del AddOn")]
        public IFormFile? AddOnImageFile { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio")]
        public AddOnType Type { get; set; }

        [Display(Name = "¿Está Activo?")]
        public bool IsActive { get; set; } = false; // Por defecto inactivo

        public virtual ICollection<TicketAddOn> TicketAddOns { get; set; } = new List<TicketAddOn>();
    }
}
