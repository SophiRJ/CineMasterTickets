using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
using CinemaMasterTicketsMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // OPCIONAL: Guardar el título y precio en Session AQUÍ 
        // para que el POST de AddOns ya no necesite recibirlos de la vista.
        HttpContext.Session.SetString("MovieTitle", vm.MovieTitle);
        HttpContext.Session.SetString("SessionTime", vm.StartTime.ToString("dd/MM/yyyy HH:mm"));

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

    //Esta es la que va desde los asientos a los addons
    [HttpPost]
    public IActionResult AddOns(int sessionId, string selectedSeats, string totalSeatsPrice)
    {
        if (string.IsNullOrEmpty(selectedSeats))
            return RedirectToAction("SeatSelection", new { sessionId });

        decimal precioLimpio = decimal.Parse(totalSeatsPrice, System.Globalization.CultureInfo.InvariantCulture);
        
        // Guardamos solo lo que cambia o es nuevo en este paso
        HttpContext.Session.SetInt32("SessionId", sessionId);
        HttpContext.Session.SetString("SelectedSeatIds", selectedSeats);

        // Guardamos el decimal como string de forma neutra
        HttpContext.Session.SetString("SeatsSubtotal", precioLimpio.ToString(System.Globalization.CultureInfo.InvariantCulture));
        Console.WriteLine($"[DEBUG] SeatsSubtotal enviado: {HttpContext.Session.GetString("SeatsSubtotal")}");
        return RedirectToAction("AddOns");
    }

    [HttpGet]
    public IActionResult AddOns()
    {
        var sessionId = HttpContext.Session.GetInt32("SessionId");
        var seatsPriceStr = HttpContext.Session.GetString("SeatsSubtotal");
        Console.WriteLine($"[DEBUG] SeatsSubtotal recuperado:: {seatsPriceStr}");

        if (sessionId == null || string.IsNullOrEmpty(seatsPriceStr))
        {
            return RedirectToAction("Index", "Home");
        }

        // 1. Convertimos el string de la sesión a decimal (usando el punto que guardamos antes)
        decimal seatsPrice = decimal.Parse(seatsPriceStr, System.Globalization.CultureInfo.InvariantCulture);

        var vm = new AddOnsViewModel
        {
            AddOnTypesSelectList = new SelectList(Enum.GetValues(typeof(AddOnType)).Cast<AddOnType>())
        };

        // 2. PASAR LIMPIO AL VIEWBAG:
        // Guardamos el decimal original para mostrarlo con coma al usuario
        ViewBag.SeatsSubtotal = seatsPrice; // Para el texto de la pantalla
        ViewBag.SeatsSubtotalRaw = seatsPrice.ToString(System.Globalization.CultureInfo.InvariantCulture); // Para el JS

        ViewBag.MovieTitle = HttpContext.Session.GetString("MovieTitle") ?? "Película";
        ViewBag.SessionTime = HttpContext.Session.GetString("SessionTime") ?? "";
        ViewBag.SessionId = sessionId;

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> GetAddOnsByType(string type)
    {
        // Obtenemos todos los addons activos
        var addons = await _db.AddOns.ToListAsync();

        // Filtramos por tipo si no es "All" ni vacío
        if (!string.IsNullOrEmpty(type) && type != "All")
        {
            if (Enum.TryParse<AddOnType>(type, out var addonType))
            {
                addons = addons.Where(a => a.Type == addonType).ToList();
            }
        }

        // Retornamos solo los campos que necesitamos para el JS
        var result = addons.Select(a => new
        {
            a.AddOnId,
            a.AddOnName,
            a.Price,
            a.AddOnImage
        });

        return Json(result);
    }
}




