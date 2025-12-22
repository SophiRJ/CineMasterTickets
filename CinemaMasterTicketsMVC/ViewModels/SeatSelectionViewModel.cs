using CinemaMasterTicketsMVC.Models;

namespace CinemaMasterTicketsMVC.ViewModels
{
    public class SeatSelectionViewModel
    {
        public int SessionId { get; set; }
        public string MovieTitle { get; set; } = "";
        public DateTime StartTime { get; set; }
        public decimal Price { get; set; }
        public int RoomId { get; set; } // El ID que tienes en tu modelo
        public List<RowSeats> SeatMap { get; set; } = new();
        //public Session Session { get; set; } = null!;
        //public List<RowSeats> SeatMap { get; set; } = new();
    }

    public class RowSeats
    {
        public string Row { get; set; } = "";
        public List<SeatInfo> Seats { get; set; } = new();
    }

    public class SeatInfo
    {
        public int SeatId { get; set; }
        public string Number { get; set; } = "";
        public bool IsReserved { get; set; }
    }
}

