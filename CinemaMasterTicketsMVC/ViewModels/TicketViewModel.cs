namespace CinemaMasterTicketsMVC.ViewModels
{
    public class TicketViewModel
    {
        public string? MovieTitle { get; set; }
        public string? SessionTime { get; set; }
        public string? RoomNumber { get; set; }

        public List<int> SelectedSeatIds { get; set; } = new List<int>();
        public List<string> SelectedSeats { get; set; } = new List<string>();
        public decimal SeatsSubtotal { get; set; }

        public List<AddonItem> Addons { get; set; } = new List<AddonItem>();

        public decimal AddonsTotal => Addons.Sum(a => a.Total);
        public decimal TotalGeneral => SeatsSubtotal + AddonsTotal;
    }

    public class AddonItem
    {
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
