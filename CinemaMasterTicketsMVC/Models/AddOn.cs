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
        public string? AddOnName { get; set; } // Corregido a PascalCase

        [Required(ErrorMessage = "Debe indicar el precio")]
        [Range(0.01, 1000, ErrorMessage = "El precio debe ser mayor que 0 y menor que 1000")]
        [Column(TypeName = "decimal(18, 2)")] // Define la precisión en la base de datos
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio")]
        public AddOnType Type { get; set; } // Corregido a PascalCase y quitado el null si es obligatorio

        
        public string? AddOnImage { get; set; }

        public virtual ICollection<TicketAddOn> TicketAddOns { get; set; } = new List<TicketAddOn>();
    }
}
