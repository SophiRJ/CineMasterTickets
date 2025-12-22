using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaMasterTicketsMVC.Models
{
    public class Movie
    {
        public int MovieId { get; set; }    

        public string? MovieAPIId { get; set; }

        public string? Title { get; set; }
        public string? Director { get; set; }
        public string? OverView { get; set; }
        public string? PosterUrl { get; set; }
        public string? BackdropUrl { get; set; }
        public int DurationMinutes { get; set; }
        public string? Genre { get; set; }
        //Funcion autoimplementada que calcula mediante un diccionario los generos que corresponden
        //a los numeros guardados en la propiedad genero, para que en vez de mostrar los id muestre 
        //los nombres directamente en la app
        [NotMapped]
        public List<string> GenreList
        {
            get
            {
                if (string.IsNullOrEmpty(Genre)) return new List<string>();

                var nombresTMDB = new Dictionary<string, string> {
                { "28", "Acción" }, { "12", "Aventura" }, { "16", "Animación" },
                { "35", "Comedia" }, { "80", "Crimen" }, { "99", "Documental" },
                { "18", "Drama" }, { "10751", "Familia" }, { "14", "Fantasía" },
                { "36", "Historia" }, { "27", "Terror" }, { "10402", "Música" },
                { "9648", "Misterio" }, { "10749", "Romance" }, { "878", "Ciencia ficción" },
                { "53", "Suspense" }, { "10752", "Bélica" }, { "37", "Western" }
            };
                //Despues de la construccion del diccionario lo que hacemos es seleccionar los id separandolos
                //por comas, y despues, coger la palabra contenida en la key con ese id, si no tiene, muestra"OTROS".
                return Genre.Split(',')
                .Select(id => id.Trim())
                .Select(id => nombresTMDB.ContainsKey(id) ? nombresTMDB[id] : "Otros")
                .ToList();
            }
        }
        public DateTime AddedAt { get; set; }

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        [NotMapped]
        public bool HasSessions => Sessions != null && Sessions.Any();
    }
}
