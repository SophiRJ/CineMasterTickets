using System.ComponentModel.DataAnnotations;

namespace CinemaMasterTicketsMVC.Models
{
    public class Customer
    {
        public int CustomerId {  get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must have at least 20 characters length")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must have at least 20 characters length")]
        public required string LastName { get; set; }
        public string? Address { get; set; }

        public string? City { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        public int FidelityPoints { get; set; }

        public bool isActive {  get; set; }


        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
