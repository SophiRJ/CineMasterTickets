namespace CinemaMasterTicketsMVC.Models
{
    public class BoxOffice
    {
        public int BoxOfficeId { get; set; }
        public string? BoxOfficeName { get; set; }
        
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
