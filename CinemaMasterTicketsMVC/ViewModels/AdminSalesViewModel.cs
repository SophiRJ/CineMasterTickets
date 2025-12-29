using CinemaMasterTicketsMVC.Models;
using X.PagedList;

//ViewModel para utilizar en la pagina de Busqueda de tickets del administrador
namespace CinemaMasterTicketsMVC.ViewModels
{

    public class AdminSalesViewModel
    {
        //Almacen para el ID para filtrar la búsqueda
        public int? SearchId { get; set; }
        //Almacen para la fecha de búsqueda
        public DateTime? SearchDate { get; set; }
        //Almacen para el precio de búsqueda
        public decimal? SearchPrice { get; set; }
        //Almacen para el usuario de búsqueda
        public string? SearchUser { get; set; }
        //Almacen para el tipo de compra en la búsqueda
        public bool? SearchBoxOffice { get; set; }

        //Bandera para saber si el boton se ha pulsado y mostrar si hay resultados o no
        public bool SearchPerformed { get; set; }

        // Almacen para guardar la lista paginada y poder meterla en el VM. Esto es necesario ya que, como 
        //no solo vamos a pasar la lista paginada, debemos guardarla en el VM para poder pasarla junto con el resto de cosas
        public IPagedList<Ticket>? FoundTickets { get; set; }

        // Colecciones de Peliculas y AddOns para poder guardar sus datos y mostrarlas en sus tablas
        public List<MovieSales> MovieSales { get; set; } = new();
        public List<AddOnSales> AddOnSales { get; set; } = new();
    }
    //Coleccion de datos necesarios para las Películas
    public class MovieSales
    {
        //Tiutlo
        public string MovieTitle { get; set; } = "";
        //Almacen para el conteo de los tickets vendidos
        public int TicketsCount { get; set; }
        //Almacen para el total de asientos vendidos
        public decimal TotalSeats { get; set; }
    }

    //Coleccion para guardar los datos necesarios de Complementos para la vista
    public class AddOnSales
    {
        //Nombre
        public string AddOnName { get; set; } = "";
        //Unidades vendidas
        public int SoldUnits { get; set; }
        //Total generado
        public decimal TotalRevenue { get; set; }
    }
}
