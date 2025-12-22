using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaMasterTicketsMVC.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int RowId { get; set; }
        public string Number { get; set; } = string.Empty;
        public bool Available { get; set; } = false;

        public Row Row { get; set; } = null!;
        public ICollection<TicketSeat> TicketSeats { get; set; } = new List<TicketSeat>();
        public ICollection<SessionSeat> SessionSeats { get; set; } = new List<SessionSeat>();

    }
}
