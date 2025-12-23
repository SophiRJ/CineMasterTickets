using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
using CinemaMasterTicketsMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
        HttpContext.Session.SetInt32("SessionId",sessionId);
        var reservedSeatIds = await _db.TicketSeats
        .Where(ts => ts.SessionId == sessionId)
        .Select(ts => ts.SeatId)
        .ToListAsync();

        // ✅ Asientos seleccionados por el usuario (Session)
        var selectedSeatIdsStr = HttpContext.Session.GetString("SelectedSeatIds");

        var selectedSeatIds = string.IsNullOrEmpty(selectedSeatIdsStr)
            ? new List<int>()
            : selectedSeatIdsStr.Split(',').Select(int.Parse).ToList();

        var vm = await _db.Sessions
            .Where(s => s.SessionId == sessionId)
            .Select(s => new SeatSelectionViewModel
            {
                SessionId = s.SessionId,
                MovieTitle = s.Movie!.Title,
                StartTime = s.StartTime,
                Price = s.Price,
                RoomId = s.RoomId,
                SeatMap = s.Room!.Rows
                    .OrderBy(r => r.Name)
                    .Select(row => new RowSeats
                    {
                        Row = row.Name.ToString(),
                        Seats = row.Seats
                            .OrderBy(seat => Convert.ToInt32(seat.Number))
                            .Select(seat => new SeatInfo
                            {
                                SeatId = seat.SeatId,
                                Number = seat.Number,
                                IsReserved = reservedSeatIds.Contains(seat.SeatId),

                                // ✅ AQUÍ ESTÁ LA CLAVE
                                IsSelected = selectedSeatIds.Contains(seat.SeatId)
                            }).ToList()
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();

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
    public IActionResult SeatSelectionPost(SeatSelectionViewModel model)
    {


        if (string.IsNullOrEmpty(model.SelectedSeats))
        {
            TempData["ErrorMessage"] = "Debes seleccionar al menos un asiento.";
            return RedirectToAction("SeatSelection", new { sessionId = model.SessionId });
        }

        HttpContext.Session.SetString("SelectedSeatIds", model.SelectedSeats);
        HttpContext.Session.SetString("SelectedSeatNames", model.SelectedSeatNames ?? "");
        HttpContext.Session.SetString("SeatUserTypes", model.SeatUserTypes ?? "");
        HttpContext.Session.SetString(
        "SeatsSubtotal",
        model.TotalSeatsPrice
    );

        return RedirectToAction("AddOns");
    }

    [HttpGet]
    public IActionResult AddOns()
    {
        var sessionId = HttpContext.Session.GetInt32("SessionId");
        var seatsPriceStr = HttpContext.Session.GetString("SeatsSubtotal");

        if (sessionId == null || string.IsNullOrEmpty(seatsPriceStr))
        {
            return RedirectToAction("Index", "Home");
        }

        // 1. Convertimos el string de la sesión a decimal (usando el punto que guardamos antes)
        decimal seatsPrice = decimal.Parse(seatsPriceStr, new CultureInfo("es-ES"));

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

