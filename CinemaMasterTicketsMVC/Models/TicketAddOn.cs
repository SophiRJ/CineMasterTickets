namespace CinemaMasterTicketsMVC.Models
{
    public class TicketAddOn
    {
        public int TicketId { get; set; }
        public int AddOnId { get; set; }

        public Ticket? Ticket { get; set; }
        public AddOn? AddOn { get; set; }
    }
}
