using System.ComponentModel.DataAnnotations;

namespace CinemaMasterTicketsMVC.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        [Required]
        public int MovieId { get; set; }
        [Required(ErrorMessage = "Debe seleccionar una sala")]
        public int RoomId { get; set; }
        [Required(ErrorMessage = "Debe indicar la hora de inicio")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "Debe indicar el precio")]
        [Range(0.01, 1000, ErrorMessage = "El precio debe ser mayor que 0 y menor que 1000")]
        public decimal Price { get; set; }
        //Por defecto cuando se crea la sesion su estado sera activo 
        public string Status { get; set; } = "Active";

        public Movie? Movie { get; set; }
        public Room? Room { get; set; }



        public ICollection<SessionSeat> SessionSeats { get; set; } = new List<SessionSeat>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
