namespace CinemaMasterTicketsMVC.Models
{
    public class AddOn
    {
        public int AddOnId { get; set; }
        
        public string? AddonName { get; set; }

        public decimal Price { get; set; }

        public ICollection<TicketAddOn> TicketAddOns { get; set; } = new List<TicketAddOn>();
    }
}
