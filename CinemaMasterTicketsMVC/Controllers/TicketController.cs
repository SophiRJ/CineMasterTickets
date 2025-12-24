using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CinemaMasterTicketsMVC.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser>? _userManager;

        public TicketController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
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

            //var seatsSubtotal = decimal.Parse(seatsSubtotalStr, System.Globalization.CultureInfo.InvariantCulture);
            //con esto funciona Rubennnn
            var seatsSubtotal = decimal.Parse(seatsSubtotalStr, new System.Globalization.CultureInfo("es-ES"));

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

            //Añadido obtener el usuario para mostrar datos del comprador
            // 1. Obtener el usuario actual
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                ViewBag.BuyerEmail = user.Email;

                // 2. Intentar buscar si es Empleado
                var employee = await _db.Employees.Include(e => e.BoxOffice)
                                    .FirstOrDefaultAsync(e => e.Email == user.Email);

                if (employee != null)
                {
                    ViewBag.BuyerName = $"{employee.Firstname} {employee.Lastname}";
                    ViewBag.BuyerRole = "Empleado Staff";
                    ViewBag.BoxOffice = employee.BoxOffice?.BoxOfficeName ?? "General";
                    ViewBag.BuyerDni = employee.DNI;
                }
                else
                {
                    // 3. Si no es empleado, buscar si es Cliente
                    var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);
                    if (customer != null)
                    {
                        ViewBag.BuyerName = $"{customer.FirstName} {customer.LastName}";
                        ViewBag.BuyerRole = "Cliente VIP";
                        ViewBag.BuyerDni = "Registrado"; // O el campo DNI si lo tienes en Customer

                        // Lógica de puntos: 1 asiento = 10 puntos
                        int puntosPorAsiento = 10;
                        int puntosAcumulados = selectedSeats.Count * puntosPorAsiento;
                        ViewBag.PuntosA_Acumular = puntosAcumulados;

                        // Guardamos en sesión para el siguiente paso (Checkout/Pago)
                        HttpContext.Session.SetInt32("PuntosPendientes", puntosAcumulados);
                    }
                    else
                    {
                        // Caso raro: tiene usuario Identity pero no está en tablas de negocio
                        ViewBag.BuyerName = user.Email;
                        ViewBag.BuyerRole = "Usuario Registrado";
                    }
                }
            }
            else
            {
                ViewBag.BuyerName = "Invitado";
                ViewBag.BuyerEmail = "Sin correo";
                ViewBag.BuyerRole = "Público General";
            }

            return View(model);
        }
    }
}

