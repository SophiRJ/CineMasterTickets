namespace CinemaMasterTicketsMVC.ViewModels
{
    //Este viewModel se encarga de recoger todos los datos dispersos en la session y que hemos 
    //ido generando a lo largo de la compra, para juntarlos todos y poder guardarlos en BBDD 
    //para asi poder generar el tiquet. Es quizá el ViewModel más completo de todo el proyecto.
    public class TicketViewModel
    {
        //Guardamos todos los datos necesarios de la sesion, pelicula, totales de asientos y addons...
        public string? MovieTitle { get; set; }
        public string? PosterURL { get; set; }
        public string? SessionTime { get; set; }
        public string? RoomNumber { get; set; }
        //Necesitamos una lista de Id de asientos seleccionados para poder guardarlos y mostrarlos
        public List<int> SelectedSeatIds { get; set; } = new List<int>();
        //Necesitamos una lista de Nombres de asientos seleccionados para poder guardarlos y mostrarlos
        public List<string> SelectedSeats { get; set; } = new List<string>();
        public decimal SeatsSubtotal { get; set; }
        //Tambien necesitamos una lista de AddOns para poder recorrerla y guardarla en BBDD para mostrarla
        public List<AddonItem> Addons { get; set; } = new List<AddonItem>();
        //Tambien una lista que recoja los datos del comprador
        public BuyerInfo Buyer { get; set; } = new BuyerInfo();
        //Esta propiedad calcula automaticamente el total de la lista de addons
        public decimal AddonsTotal => Addons.Sum(a => a.Total);
        // Si el usuario canjea puntos, esta propiedad contendra el descuento de esos puntos
        public decimal DescuentoPuntos { get; set; } = 0; 
        //Aqui guardamos el total general
        public decimal TotalGeneral => (SeatsSubtotal + AddonsTotal) - DescuentoPuntos;
    }

    //Aqui guardamos cada item con sus propiedades
    public class AddonItem
    {
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        //Propiedad que calcula automaticamente precio * cantidad
        public decimal Total => Quantity * UnitPrice;
    }

    //datos del comprador
    public class BuyerInfo
    {
        public int? BuyerId { get; set; } // El ID de la tabla Customer o Employee
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? BoxOffice { get; set; }

        // Datos de fidelidad
        public int PointsToEarn { get; set; }
        public int CurrentFidelityPoints { get; set; } // Los que ya tiene en BD

        // Propiedades de control claras
        public bool IsCustomer { get; set; }
        public bool IsEmployee { get; set; }
    }
}
