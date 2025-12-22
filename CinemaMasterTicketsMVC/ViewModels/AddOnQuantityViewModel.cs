using CinemaMasterTicketsMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaMasterTicketsMVC.ViewModels
{
    public class AddOnsViewModel
    {
        public string? SelectedType { get; set; }
        public SelectList? AddOnTypesSelectList { get; set; }
    }
}
