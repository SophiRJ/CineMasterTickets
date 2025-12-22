namespace CinemaMasterTicketsMVC.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public int Capacity { get; set; }

        public ICollection<Row> Rows { get; set; } = new List<Row>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        //Metodo que valida si la sala esta disponible 
        //sera llamado al crear la nueva sesion
        public bool EstaDisponible(DateTime inicio, TimeSpan duracion)
        {
            return !Sessions
                .Where(s => s.Status == "Active" && s.Movie != null)
                .Any(s =>
                    inicio < s.StartTime.Add(TimeSpan.FromMinutes(s.Movie!.DurationMinutes)) &&
                    inicio.Add(duracion) > s.StartTime
                );
        }

    }
}
