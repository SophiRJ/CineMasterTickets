namespace CinemaMasterTicketsMVC.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public int? CustomerId { get; set; }
        public int? EmployeeId { get; set; }

        public string? EmailToSend { get; set; }
        public int SessionId { get; set; }
        public decimal TotalPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? PurchasedAt { get; set; } = DateTime.Now;
        public bool SoldAtBoxOffice { get; set; }

        public Customer? Customer { get; set; }
        public Employee? Employee { get; set; }
        public Session? Session { get; set; }

        public ICollection<TicketSeat> TicketSeats { get; set; } = new List<TicketSeat>();
        public ICollection<TicketAddOn> TicketAddOns { get; set; } = new List<TicketAddOn>();
    }
}
