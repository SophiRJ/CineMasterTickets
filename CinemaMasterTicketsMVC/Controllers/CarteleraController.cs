using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaMasterTicketsMVC.Controllers
{
    public class CarteleraController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CarteleraController(ApplicationDbContext db)
        {
            _db = db;
        }
        //Este metodo carga la pantalla principal de la cartelera y gesitona
        //el estado de las sesiones
        public IActionResult Index()
        {
            //Antes de mostrar la cartelera, buscamos sesiones que sigan como "Active"
            // pero que su hora de inicio ya haya pasado.
            var toFinish = _db.Sessions
                .Where(s => s.Status == "Active" && s.StartTime < DateTime.Now);


            //LAs marcamos como finalizadas para que ya no se muestren en la cartelera
            foreach (var s in toFinish)
                s.Status = "Finished";
            _db.SaveChanges();

            //Cargamos las peliculas ordenadas por las mas recientes e incluimos sus sesiones
            var movies = _db.Movies
                .Include(m => m.Sessions)
                .OrderByDescending(m => m.AddedAt)
                .ToList();


            //Filtramos las sesiones de cada pelicula para mostrar solo las que estan activas
            foreach (var movie in movies)
            {
                movie.Sessions = movie.Sessions
                    .Where(s => s.Status == "Active")
                    .OrderBy(s => s.StartTime)
                    .ToList();
            }

            //Cargar salas para el dropdown
            ViewBag.Rooms = _db.Rooms.ToList();
            return View(movies);
        }

        //Este metodo devuelve todas las sesiones de una pelicula especifica se llama desde JS
        [HttpGet]
        public async Task<IActionResult> GetSessions(int movieId)
        {
            var sessions = await _db.Sessions
                .AsNoTracking() //mejora rendimiento para datos d lectura
                .Where(s => s.MovieId == movieId && s.Status == "Active")
                .OrderBy(s => s.StartTime)
                .ToListAsync();
            //Devolvemos la vista parcial para que js la inyecte sin recargar toda la pagina
            return PartialView("_SessionsPartial", sessions);
        }



        
        //metodo controla que la sala este diponible en base a la duracion de la pelicula, si hay alguna sesion creada
        //en esa sala en la misma fecha y hora que se este asignando
        //la duracion en minutos se extrae desde la movie
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] Session session)
        {
            //Validacion del modelo
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener la película para saber cuanto dura en minutos
            var movie = await _db.Movies
                .FirstOrDefaultAsync(m => m.MovieId == session.MovieId);

            if (movie == null)
                return NotFound("La película no existe");

            // Obtenemos la sala con sus sesiones
            var room = await _db.Rooms
                .Include(r => r.Sessions)
                    .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(r => r.RoomId == session.RoomId);

            if (room == null)
                return NotFound("La sala no existe");

            // Duración en pelicula-> aqui se saca la duracion de la pelicula desde la base de datos
            TimeSpan duracion = TimeSpan.FromMinutes(movie.DurationMinutes);

            // Validar disponibilidad
            if (!room.EstaDisponible(session.StartTime, duracion))
            {
                return BadRequest("La sala no está disponible en esa fecha y hora");
            }

            session.Status = "Active"; //Cambiar el estado de la sesion para controlar si aparece o no en a cartelera

            // Guardar sesión
            _db.Sessions.Add(session);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Metodo para editar una sesion existente controlando tambien solapamientos de sesiones
        [HttpPost]
        public async Task<IActionResult> UpdateSession([FromBody] Session session)
        {
            var existing = await _db.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.SessionId == session.SessionId);

            if (existing == null)
                return NotFound();

            TimeSpan duracion = TimeSpan.FromMinutes(existing.Movie!.DurationMinutes);

            // Cargar la sala con todas sus sesiones activas, incluyendo las películas
            var room = await _db.Rooms
                .Include(r => r.Sessions)
                    .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(r => r.RoomId == session.RoomId);

            if (room == null)
                return NotFound("Sala no encontrada");

            // Excluir la sesión que estamos editando del chequeo, por que si no el sistema dira que no
            // esta disponible ya que choca consigo misma
            bool disponible = room.Sessions
                .Where(s => s.Status == "Active" && s.SessionId != existing.SessionId && s.Movie != null)
                .All(s =>
                    session.StartTime >= s.StartTime.Add(TimeSpan.FromMinutes(s.Movie!.DurationMinutes)) ||
                    session.StartTime.Add(duracion) <= s.StartTime
                );

            if (!disponible)
                return BadRequest("Sala no disponible");
            //Actualizamos solo los campos permitidos
            existing.StartTime = session.StartTime;
            existing.RoomId = session.RoomId;
            existing.Price = session.Price;

            await _db.SaveChangesAsync();
            return Ok();
        }

        //Mtodo para eliminar sesiones que no tengan tickets vendidos
        [HttpPost]
        public async Task<IActionResult> DeleteSession(int sessionId)
        {
            var session = await _db.Sessions
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound();

            //Si hay tickets vendidos no se puede borrar la sesion ya que se dejaria tickets sin referencia 
            //y romperiamos la estructura definida.
            if (session.Tickets.Any())
                return BadRequest("No se puede borrar una sesión con tickets vendidos");

            _db.Sessions.Remove(session);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Cancelar sesión-> este metodo mantiene el registro pero la quita de la venta
        //Al cabiar el estado los tickets vendidos quedan no quedan huerfanos sino quedan asociados
        //a una sesion "Cancelled"
        [HttpPost]
        public async Task<IActionResult> CancelSession(int sessionId)
        {
            var session = await _db.Sessions.FindAsync(sessionId);
            if (session == null)
                return NotFound();

            session.Status = "Cancelled";
            await _db.SaveChangesAsync();

            return Ok();
        }

        // este metodo sirve para eliminar película solo si no tiene sesiones
        [HttpPost]
        public async Task<IActionResult> DeleteMovie(int movieId)
        {
            var movie = await _db.Movies
                .Include(m => m.Sessions)
                .ThenInclude(s => s.SessionSeats) //Incluimos los asientos de la sesión
                .FirstOrDefaultAsync(m => m.MovieId == movieId);

            if (movie == null)
                return NotFound();

            // Si tiene sesiones activas no finalizadas no permitimos borrar
            if (movie.Sessions.Any(s => s.Status == "Active"))
                return BadRequest("La película tiene sesiones activas");

            // borramos los bloques de asientos de todas sus sesiones para que la base de datos no de error 
            //en las claves foraneas
            foreach (var session in movie.Sessions)
            {
                _db.SessionSeats.RemoveRange(session.SessionSeats);
            }

            // Borrar las sesiones
            _db.Sessions.RemoveRange(movie.Sessions);

            //Borrar la pelicula
            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();

            return Ok();
        }

    }
}

