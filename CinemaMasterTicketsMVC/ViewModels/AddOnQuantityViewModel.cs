using CinemaMasterTicketsMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
//Viewmodel que simplemente guarda el tipo seleccionado y una lista lseleccionable para construir el 
//Select de Addons en la compra.
namespace CinemaMasterTicketsMVC.ViewModels
{
    public class AddOnsViewModel
    {
        public string? SelectedType { get; set; }
        public SelectList? AddOnTypesSelectList { get; set; }
    }
}
