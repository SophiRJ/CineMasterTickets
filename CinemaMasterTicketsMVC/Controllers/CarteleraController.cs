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

        public IActionResult Index()
        {

            // limpiar sesiones pasadas
            //Marcar sesiones pasadas como Finalizadas-> se ca,bia su estado
            var toFinish = _db.Sessions
                .Where(s => s.Status == "Active" && s.StartTime < DateTime.Now);

            foreach (var s in toFinish)
                s.Status = "Finished";
            _db.SaveChanges();

            var movies = _db.Movies
                .Include(m => m.Sessions) //Cargar peliculas con sesiones
                .OrderByDescending(m => m.AddedAt)
                .ToList();

            //Cargar salas para el dropdown
            ViewBag.Rooms = _db.Rooms.ToList();
            return View(movies);
        }
        //Obtener sesiones por pelicula

        [HttpGet]
        public async Task<IActionResult> GetSessions(int movieId)
        {
            var sessions = await _db.Sessions
                .Where(s => s.MovieId == movieId && s.Status == "Active")
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return PartialView("_SessionsPartial", sessions);
        }
        //Obtener Sesiones activas en general
        //public async Task<IActionResult> ActiveSessions()
        //{
        //    var sessions = await _db.Sessions
        //        .Where(s => s.Status == "Active")
        //        .Include(s=>s.Movie)
        //        .Include(s => s.Room)
        //        .OrderBy(s => s.StartTime)
        //        .ToListAsync();
        //    return PartialView("_ActiveSessionsTable", sessions);
        //}


        //modificado 19/12/2025
        //metodo controla que la sala este diponible en base a la duracion de la pelicula, si hay alguna sesion creada
        //en esa sala en la misma fecha y hora que se este asignando
        //la duracion en minutos se extrae desde la movie
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] Session session)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener la película
            var movie = await _db.Movies
                .FirstOrDefaultAsync(m => m.MovieId == session.MovieId);

            if (movie == null)
                return NotFound("La película no existe");

            // Obtener la sala con sus sesiones
            //var room = await _db.Rooms
            //    .Include(r => r.Sessions)
            //    .FirstOrDefaultAsync(r => r.RoomId == session.RoomId);

            var room = await _db.Rooms
                .Include(r => r.Sessions)
                    .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(r => r.RoomId == session.RoomId);

            if (room == null)
                return NotFound("La sala no existe");

            // Duración en pelicula-> aqui se saca la duracion de la pelicula desde la base de datos
            TimeSpan duracion = TimeSpan.FromMinutes(movie.DurationMinutes);

            // Duracion predeterminada-> PARA PRUEBAS
            //TimeSpan duracion = TimeSpan.FromHours(3);

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

            //var room = await _db.Rooms
            //    .Include(r => r.Sessions)
            //    .FirstAsync(r => r.RoomId == session.RoomId);
            ////var room = await _db.Rooms
            ////    .Include(r => r.Sessions)
            ////        .ThenInclude(s => s.Movie)
            ////    .FirstOrDefaultAsync(r => r.RoomId == session.RoomId);

            //if (room.EstaDisponible(session.StartTime, duracion))
            //    return BadRequest("Sala no disponible");
            // Cargar la sala con todas sus sesiones activas, incluyendo las películas
            var room = await _db.Rooms
                .Include(r => r.Sessions)
                    .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(r => r.RoomId == session.RoomId);

            if (room == null)
                return NotFound("Sala no encontrada");

            // Excluir la sesión que estamos editando del chequeo
            bool disponible = room.Sessions
                .Where(s => s.Status == "Active" && s.SessionId != existing.SessionId && s.Movie != null)
                .All(s =>
                    session.StartTime >= s.StartTime.Add(TimeSpan.FromMinutes(s.Movie!.DurationMinutes)) ||
                    session.StartTime.Add(duracion) <= s.StartTime
                );

            if (!disponible)
                return BadRequest("Sala no disponible");

            existing.StartTime = session.StartTime;
            existing.RoomId = session.RoomId;
            existing.Price = session.Price;

            await _db.SaveChangesAsync();
            return Ok();
        }

        //Eliminar sesiones que no tengan tickets vendidos
        [HttpPost]
        public async Task<IActionResult> DeleteSession(int sessionId)
        {
            var session = await _db.Sessions
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return NotFound();

            if (session.Tickets.Any())
                return BadRequest("No se puede borrar una sesión con tickets vendidos");

            _db.Sessions.Remove(session);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //Cancelar sesión
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

        // Eliminar película (solo si no tiene sesiones)
        [HttpPost]
        public async Task<IActionResult> DeleteMovie(int movieId)
        {
            var movie = await _db.Movies
                .Include(m => m.Sessions)
                .FirstOrDefaultAsync(m => m.MovieId == movieId);

            if (movie == null)
                return NotFound();

            if (movie.Sessions.Any(s => s.Status == "Active"))
                return BadRequest("La película tiene sesiones activas");

            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();

            return Ok();
        }

    }
}

