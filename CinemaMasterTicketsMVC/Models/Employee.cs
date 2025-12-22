using System.ComponentModel.DataAnnotations;

namespace CinemaMasterTicketsMVC.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(20,MinimumLength=3,ErrorMessage ="Name must have at least 20 characters length")]
        public string? Firstname { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must have at least 20 characters length")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "DNI required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "DNI must have at least 10 characters length")]
        [RegularExpression(@"^[A-Za-z0-9-]{10}$", ErrorMessage = "DNI only can contains numbers, characters and '-'")]
        public string? DNI { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public int BoxOfficeId { get; set; }

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public BoxOffice? BoxOffice { get; set; }
    }
}
