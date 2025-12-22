namespace CinemaMasterTicketsMVC.Models
{
    public class TicketSeat
    {
        public int TicketId { get; set; }
        public int SeatId { get; set; }
        public int SessionId { get; set; }
        public Ticket? Ticket { get; set; }
        public Seat? Seat { get; set; }
        public Session? Session { get; set; }
    }
}
