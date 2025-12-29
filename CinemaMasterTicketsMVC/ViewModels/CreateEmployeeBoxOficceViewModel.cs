using CinemaMasterTicketsMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

//ViewModel para meter un objeto de empleado junto con una lista de puestos de
//trabajo en la creacion de empleado
namespace CinemaMasterTicketsMVC.ViewModels
{
    public class CreateEmployeeBoxOficceViewModel
    {
        public Employee? Employee { get; set; }
        public SelectList? BoxOffices { get; set; }
    }
}
