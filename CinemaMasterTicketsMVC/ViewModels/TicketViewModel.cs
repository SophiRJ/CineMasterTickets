namespace CinemaMasterTicketsMVC.ViewModels
{
    public class TicketViewModel
    {
        public string? MovieTitle { get; set; }
        public string? SessionTime { get; set; }
        public string? RoomNumber { get; set; }

        public List<int> SelectedSeatIds { get; set; } = new List<int>();
        public List<string> SelectedSeats { get; set; } = new List<string>();
        public decimal SeatsSubtotal { get; set; }

        public List<AddonItem> Addons { get; set; } = new List<AddonItem>();
        //Añadido-> datos del comprador
        public BuyerInfo Buyer { get; set; } = new BuyerInfo();

        public decimal AddonsTotal => Addons.Sum(a => a.Total);
        //public decimal TotalGeneral => SeatsSubtotal + AddonsTotal;
        //Añadido-> descontar puntos y ajustar el total general
        public decimal DescuentoPuntos { get; set; } = 0; // Se llenará si el usuario canjea
        public decimal TotalGeneral => (SeatsSubtotal + AddonsTotal) - DescuentoPuntos;
    }

    public class AddonItem
    {
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
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
        public int PointsToEarn { get; set; }  // +10 por asiento
        public int CurrentFidelityPoints { get; set; } // Los que ya tiene en BD

        // Propiedades de control claras
        public bool IsCustomer { get; set; }
        public bool IsEmployee { get; set; }
    }
}
