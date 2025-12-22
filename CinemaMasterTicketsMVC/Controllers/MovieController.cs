using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
using CinemaMasterTicketsMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaMasterTicketsMVC.Controllers
{
      
    }
    public class MovieController : Controller
    {
    private readonly ApplicationDbContext _db;
    public MovieController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int id)
    {
        HttpContext.Session.SetInt32("MovieId", id);

        var movie = await _db.Movies
            .Include(m => m.Sessions)
                .ThenInclude(s => s.Tickets)
                    .ThenInclude(t => t.TicketSeats)
            .Include(m => m.Sessions)
                .ThenInclude(s => s.Room)
                    .ThenInclude(r => r!.Rows)
                        .ThenInclude(row => row.Seats)
            .FirstOrDefaultAsync(m => m.MovieId == id);

        if (movie == null)
            return NotFound();

        // Agrupamos sesiones por día
        var sessionsByDay = movie.Sessions.Where(s => s.MovieId == id && s.Status == "Active")
            .OrderBy(s => s.StartTime)
            .GroupBy(s => s.StartTime.Date)
            .Select(g => new
            {
                Day = g.Key,
                Sessions = g.Select(s =>
                {
                    // Total de asientos de la sala sumando todas las filas
                    int totalSeats = s.Room!.Rows.Sum(r => r.Seats.Count);

                    // Asientos reservados en esta sesión
                    int reservedSeats = s.Tickets
                        .SelectMany(t => t.TicketSeats)
                        .Count();

                    return new
                    {
                        Session = s,
                        HasAvailableSeats = reservedSeats < totalSeats
                    };
                }).ToList()
            })
            .ToList();

        ViewBag.SessionsByDay = sessionsByDay;

        return View(movie);
    }

    public IActionResult BackToHome()
    {
        // Borramos TODOS los datos de la session para evitar errores
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> SeatSelection(int sessionId)
    {
        var reservedSeatIds = await _db.TicketSeats
        .Where(ts => ts.SessionId == sessionId)
        .Select(ts => ts.SeatId)
        .ToListAsync();

        var vm = await _db.Sessions
            .Where(s => s.SessionId == sessionId)
            .Select(s => new SeatSelectionViewModel
            {
                SessionId = s.SessionId,
                MovieTitle = s.Movie!.Title,
                StartTime = s.StartTime,
                Price = s.Price,
                RoomId = s.RoomId,
                // 1. Ordenamos las filas (por nombre: A, B, C...)
                SeatMap = s.Room!.Rows.OrderBy(r => r.Name).Select(row => new RowSeats
                {
                    Row = row.Name.ToString(),
                    // 2. Ordenamos los asientos dentro de la fila (por número: 1, 2, 3...)
                    Seats = row.Seats
                        .OrderBy(seat => Convert.ToInt32(seat.Number)) 
                        .Select(seat => new SeatInfo
                        {
                            SeatId = seat.SeatId,
                            Number = seat.Number, 
                            IsReserved = reservedSeatIds.Contains(seat.SeatId)
                        }).ToList()
                    }).ToList()
                })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();

        return View(vm);
    }

    public IActionResult CancelPurchase()
    {
        var movieId = HttpContext.Session.GetInt32("MovieId");

        HttpContext.Session.Remove("SessionId");
        HttpContext.Session.Remove("SelectedSeatIds");

        if (movieId.HasValue)
        {
            return RedirectToAction("Index", "Movie", new { id = movieId.Value });
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult AddOns(int sessionId, string selectedSeats)
    {
        if (string.IsNullOrEmpty(selectedSeats))
            return RedirectToAction("SeatSelection", new { sessionId });

        // Guardamos los datos en la sesión
        HttpContext.Session.SetInt32("SessionId", sessionId);
        HttpContext.Session.SetString("SelectedSeatIds", selectedSeats);

        return RedirectToAction("AddOns"); // Redirigimos al GET
    }

    [HttpGet]
    public async Task<IActionResult> AddOns()
    {
        // Recuperamos los datos de la sesión
        var sessionId = HttpContext.Session.GetInt32("SessionId");
        var seatIdsRaw = HttpContext.Session.GetString("SelectedSeatIds");

        if (sessionId == null || string.IsNullOrEmpty(seatIdsRaw))
            return RedirectToAction("Index", "Home"); // O a la selección de películas

        var seatIds = seatIdsRaw.Split(',').Select(int.Parse).ToList();

        // Reutilizamos tu ViewModel para mostrar el resumen
        var vm = await _db.Sessions
            .Where(s => s.SessionId == sessionId)
            .Select(s => new SeatSelectionViewModel
            {
                SessionId = s.SessionId,
                MovieTitle = s.Movie!.Title,
                StartTime = s.StartTime,
                Price = s.Price,
                RoomId = s.RoomId,
                // Aquí cargamos solo los asientos que el usuario seleccionó
                SeatMap = s.Room!.Rows.Select(row => new RowSeats
                {
                    Row = row.Name.ToString(),
                    Seats = row.Seats
                        .Where(st => seatIds.Contains(st.SeatId))
                        .Select(st => new SeatInfo
                        {
                            SeatId = st.SeatId,
                            Number = st.Number
                        }).ToList()
                }).Where(r => r.Seats.Any()).ToList()
            }).FirstOrDefaultAsync();

        return View(vm);
    }
}




