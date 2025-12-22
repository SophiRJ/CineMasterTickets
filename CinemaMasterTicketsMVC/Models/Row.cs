namespace CinemaMasterTicketsMVC.Models
{
    public class Row
    {
        public int RowId { get; set; }
        public int RoomId { get; set; }
        public char Name { get; set; }

        public Room? Room { get; set; }
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
