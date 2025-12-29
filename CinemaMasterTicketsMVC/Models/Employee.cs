using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaMasterTicketsMVC.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 20 caracteres")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El apellido debe tener entre 3 y 20 caracteres")]
        public string? Lastname { get; set; }

       
        [Required(ErrorMessage = "El DNI es obligatorio")]
        // Validamos la longitud exacta de 9 caracteres (8 números + 1 letra)
        [MinLength(9, ErrorMessage = "El DNI debe tener 8 números y una letra (9 caracteres en total)")]
        [MaxLength(9, ErrorMessage = "El DNI debe tener 8 números y una letra (9 caracteres en total)")]
        // La expresión regular ahora obliga a: 8 dígitos (\d{8}) y una letra ([A-Za-z]) al final ($)
        [RegularExpression(@"^\d{8}[A-Za-z]$", ErrorMessage = "Formato de DNI inválido. Ejemplo: 12345678A")]
        public string? DNI { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Por favor, introduce una dirección de correo válida")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Debe asignar una taquilla al empleado")]
        public int BoxOfficeId { get; set; }
       
        public string? ProfileImage { get; set; } // Ruta en BD
        public bool isActive { get; set; } = true;
        [NotMapped]
        public IFormFile? ProfileImageFile { get; set; } // Solo para subida

        [NotMapped]
        public int VentasHoy => TicketsHoy.Count(); //usa la lista de abajo

        [NotMapped]
        public IEnumerable<Ticket> TicketsHoy => Tickets?
            .Where(t => t.PurchasedAt.HasValue && t.PurchasedAt.Value.Date == DateTime.Today)
            .OrderByDescending(t => t.PurchasedAt) ?? Enumerable.Empty<Ticket>();

        // ---------------------

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public BoxOffice? BoxOffice { get; set; }
    }
}
