using CinemaMasterTicketsMVC.Models;

namespace CinemaMasterTicketsMVC.ViewModels
{
    //view model para mostrar el panel de usuario del admin
    public class AdminUsersPanelViewModel
    {
        public List<Employee> Employees { get; set; } = new();
        public List<Customer> Customers { get; set; } = new();
    }
}
