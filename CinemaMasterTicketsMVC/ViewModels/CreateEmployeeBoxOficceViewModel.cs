using CinemaMasterTicketsMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaMasterTicketsMVC.ViewModels
{
    public class CreateEmployeeBoxOficceViewModel
    {
        public Employee? Employee { get; set; }
        public SelectList? BoxOffices { get; set; }
    }
}
