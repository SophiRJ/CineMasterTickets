using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaMasterTicketsMVC.Models
{
    public class Customer
    {
        public int CustomerId {  get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 20 caracteres.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El apellido debe tener entre 3 y 20 caracteres.")]
        public required string LastName { get; set; }

        [StringLength(100, ErrorMessage = "La dirección es demasiado larga.")]
        [RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9\s,.'#-]+$",
            ErrorMessage = "La dirección no puede contener solo números y debe ser válida.")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "El nombre de la ciudad es demasiado largo.")]
        [RegularExpression(@"^[a-zA-Z\sñÑáéíóúÁÉÍÓÚ]+$",
            ErrorMessage = "La ciudad solo puede contener letras.")]
        public string? City { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        public int FidelityPoints { get; set; }

        //PROPIEDAD QUE NO SE USA
        public bool isActive {  get; set; } = true;

        // ruta de la imagen (nullable)
        public string? ProfileImage { get; set; }

        // no persistido (solo para subir)
        [NotMapped]
        public IFormFile? ProfileImageFile { get; set; }


        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
