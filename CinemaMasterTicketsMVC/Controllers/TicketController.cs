using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CinemaMasterTicketsMVC.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _db;

        public TicketController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Index(string addonsData)
        {
            if (!string.IsNullOrEmpty(addonsData))
            {
                HttpContext.Session.SetString("AddonsData", addonsData);
            }

            // Recuperar datos de la sesión
            var sessionId = HttpContext.Session.GetInt32("SessionId");
            var seatIdsStr = HttpContext.Session.GetString("SelectedSeatIds");
            var seatsSubtotalStr = HttpContext.Session.GetString("SeatsSubtotal");
            var selectedSeatNamesStr = HttpContext.Session.GetString("SelectedSeatNames"); 

            if (sessionId == null || string.IsNullOrEmpty(seatIdsStr)
                || string.IsNullOrEmpty(seatsSubtotalStr)
                || string.IsNullOrEmpty(selectedSeatNamesStr))
            {
                return RedirectToAction("Index", "Home");
            }

            var seatsSubtotal = decimal.Parse(seatsSubtotalStr, System.Globalization.CultureInfo.InvariantCulture);

            // Obtener datos completos de la sesión y sala
            var session = await _db.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null) return RedirectToAction("Index", "Home");

            // ✅ Usamos directamente los nombres de los asientos desde la sesión
            var selectedSeats = selectedSeatNamesStr.Split(',').ToList();

            // Procesar addons
            var addonQuantitiesStr = HttpContext.Session.GetString("AddonsData");
            var addonQuantities = string.IsNullOrEmpty(addonQuantitiesStr)
                ? new Dictionary<int, int>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(addonQuantitiesStr)!;

            var addons = await _db.AddOns.ToListAsync();

            var selectedAddons = addons
                .Where(a => addonQuantities.ContainsKey(a.AddOnId))
                .Select(a => new AddonItem
                {
                    Name = a.AddOnName,
                    UnitPrice = a.Price,
                    Quantity = addonQuantities[a.AddOnId]
                })
                .ToList();

            var totalAddons = selectedAddons.Sum(a => a.Total);
            var grandTotal = seatsSubtotal + totalAddons;

            // Construir ViewModel
            var model = new TicketViewModel
            {
                MovieTitle = session.Movie!.Title,
                RoomNumber = session.Room.RoomId.ToString(),
                SessionTime = session.StartTime.ToString("dd/MM/yyyy HH:mm"),
                SelectedSeats = selectedSeats, // <-- aquí usamos la sesión
                SeatsSubtotal = seatsSubtotal,
                Addons = selectedAddons
            };

            return View(model);
        }
    }
}

