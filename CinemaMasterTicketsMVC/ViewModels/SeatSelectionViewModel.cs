using CinemaMasterTicketsMVC.Models;
//Este ViewModel servira de molde para pasarle informacion
//a la pantalla de seleccion de butacas (y para guardarla)
namespace CinemaMasterTicketsMVC.ViewModels
{
    public class SeatSelectionViewModel
    {//Almacenes para guardar datos de sesion
        public int SessionId { get; set; }
        public string MovieTitle { get; set; } = "";
        public DateTime StartTime { get; set; }
        public decimal Price { get; set; }
        public int RoomId { get; set; }
        //Lista que contendra la info de los asientos para que la vista los pinte como reservados, libres etc...
        //Y para que tambien los pase al siguiente action para guardarlos en Session
        public List<RowSeats> SeatMap { get; set; } = new();
        public string? SelectedSeats { get; set; }
        public string? SelectedSeatNames { get; set; }
        public string? SeatUserTypes { get; set; }
        public string? TotalSeatsPrice { get; set; }
    }
    //En la lista de asientos guardamos la fila junto con un numero de asientos/fila
    public class RowSeats
    {
        public string Row { get; set; } = "";
        public List<SeatInfo> Seats { get; set; } = new();
    }

    //Aqui guardamos la info de cada asiento (Id, Numero, Reservado, Seleccionado?, Tipo de usuario)
    public class SeatInfo
    {
        public int SeatId { get; set; }
        public string Number { get; set; } = "";
        public bool IsReserved { get; set; }
        //Este sirve para marcar los asientos seleccionados que tiene el usuario por si vuelve atras
        public bool IsSelected { get; set; }
        public string UserType { get; set; } = "Adulto"; // valor por defecto
    }
}

