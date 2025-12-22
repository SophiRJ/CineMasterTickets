namespace CinemaMasterTicketsMVC.Models
{
    public class SessionSeat
    {
        public int SessionId { get; set; }
        public int SeatId { get; set; }

        public Session? Session { get; set; }
        public Seat? Seat { get; set; }
    }
}
