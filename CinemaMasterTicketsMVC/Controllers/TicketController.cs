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

        //[HttpPost]
        //public async Task<IActionResult> Index(string addonsData)
        //{
        //    if (!string.IsNullOrEmpty(addonsData))
        //    {
        //        HttpContext.Session.SetString("AddonsData", addonsData);
        //    }

        //    // Recuperar datos de la sesión
        //    var sessionId = HttpContext.Session.GetInt32("SessionId");
        //    var seatIdsStr = HttpContext.Session.GetString("SelectedSeatIds");
        //    var seatsSubtotalStr = HttpContext.Session.GetString("SeatsSubtotal");
        //    var selectedSeatNamesStr = HttpContext.Session.GetString("SelectedSeatNames"); 

        //    if (sessionId == null || string.IsNullOrEmpty(seatIdsStr)
        //        || string.IsNullOrEmpty(seatsSubtotalStr)
        //        || string.IsNullOrEmpty(selectedSeatNamesStr))
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    //var seatsSubtotal = decimal.Parse(seatsSubtotalStr, System.Globalization.CultureInfo.InvariantCulture);
        //    //con esto funciona Rubennnn
        //    var seatsSubtotal = decimal.Parse(seatsSubtotalStr, new System.Globalization.CultureInfo("es-ES"));

        //    // Obtener datos completos de la sesión y sala
        //    var session = await _db.Sessions
        //        .Include(s => s.Movie)
        //        .Include(s => s.Room)
        //        .FirstOrDefaultAsync(s => s.SessionId == sessionId);

        //    if (session == null) return RedirectToAction("Index", "Home");

        //    // ✅ Usamos directamente los nombres de los asientos desde la sesión
        //    var selectedSeats = selectedSeatNamesStr.Split(',').ToList();

        //    // Procesar addons
        //    var addonQuantitiesStr = HttpContext.Session.GetString("AddonsData");
        //    var addonQuantities = string.IsNullOrEmpty(addonQuantitiesStr)
        //        ? new Dictionary<int, int>()
        //        : System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(addonQuantitiesStr)!;

        //    var addons = await _db.AddOns.ToListAsync();

        //    var selectedAddons = addons
        //        .Where(a => addonQuantities.ContainsKey(a.AddOnId))
        //        .Select(a => new AddonItem
        //        {
        //            Name = a.AddOnName,
        //            UnitPrice = a.Price,
        //            Quantity = addonQuantities[a.AddOnId]
        //        })
        //        .ToList();

        //    var totalAddons = selectedAddons.Sum(a => a.Total);
        //    var grandTotal = seatsSubtotal + totalAddons;

        //    // Construir ViewModel
        //    var model = new TicketViewModel
        //    {
        //        MovieTitle = session.Movie!.Title,
        //        RoomNumber = session.Room.RoomId.ToString(),
        //        SessionTime = session.StartTime.ToString("dd/MM/yyyy HH:mm"),
        //        SelectedSeats = selectedSeats, // <-- aquí usamos la sesión
        //        SeatsSubtotal = seatsSubtotal,
        //        Addons = selectedAddons,
        //        Buyer = new BuyerInfo() // Inicializamos
        //    };

        //    var user = await _userManager.GetUserAsync(User);
        //    if (user != null)
        //    {
        //        model.Buyer.Email = user.Email;

        //        var employee = await _db.Employees.Include(e => e.BoxOffice)
        //                                .FirstOrDefaultAsync(e => e.Email == user.Email);
        //        if (employee != null)
        //        {
        //            model.Buyer.BuyerId = employee.EmployeeId;
        //            model.Buyer.Name = $"{employee.Firstname} {employee.Lastname}";
        //            model.Buyer.Role = "Empleado Staff";
        //            model.Buyer.BoxOffice = employee.BoxOffice?.BoxOfficeName;
        //            model.Buyer.IsEmployee = true;

        //            // GUARDAMOS ID DE EMPLEADO EN SESIÓN
        //            HttpContext.Session.SetInt32("EmployeeId", employee.EmployeeId);
        //        }
        //        else
        //        {
        //            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);
        //            if (customer != null)
        //            {
        //                model.Buyer.BuyerId = customer.CustomerId;
        //                model.Buyer.Name = $"{customer.FirstName} {customer.LastName}";
        //                model.Buyer.Role = "Cliente VIP";
        //                model.Buyer.IsCustomer = true;
        //                model.Buyer.CurrentFidelityPoints = customer.FidelityPoints;
        //                model.Buyer.PointsToEarn = selectedSeats.Count * 10;

        //                // GUARDAMOS DATOS DEL CLIENTE Y PUNTOS EN SESIÓN
        //                HttpContext.Session.SetInt32("BuyerId", customer.CustomerId);
        //                HttpContext.Session.SetInt32("PuntosActuales", customer.FidelityPoints);
        //                HttpContext.Session.SetInt32("PuntosPendientes", model.Buyer.PointsToEarn);
        //            }
        //        }
        //    }

        //    // SI YA SE APLICÓ UN DESCUENTO (para refrescar la vista si canjea puntos)
        //    var descuentoStr = HttpContext.Session.GetString("DescuentoAplicado");
        //    if (!string.IsNullOrEmpty(descuentoStr))
        //    {
        //        model.DescuentoPuntos = decimal.Parse(descuentoStr, System.Globalization.CultureInfo.InvariantCulture);
        //    }

        //    return View(model);
        //}


        ////metodo para aplicar descuento
        //[HttpPost]
        //public IActionResult ApplyPoints()
        //{
        //    var puntosActuales = HttpContext.Session.GetInt32("PuntosActuales") ?? 0;

        //    if (puntosActuales >= 100)
        //    {
        //        // Regla: Por cada 100 puntos, 1€ de descuento
        //        // Puedes cambiar esto según tu lógica
        //        decimal descuento = 1.00m;

        //        // Guardamos que se usaron 100 puntos
        //        HttpContext.Session.SetInt32("PuntosACanjear", 100);
        //        HttpContext.Session.SetString("DescuentoAplicado", descuento.ToString(System.Globalization.CultureInfo.InvariantCulture));
        //    }

        //    return RedirectToAction("Index"); // Recarga la página para mostrar el nuevo Total
        //}
        // --- 1. EL POST (Para los datos de tu compañero) ---
        [HttpPost]
        public IActionResult Index(string addonsData)
        {
            if (!string.IsNullOrEmpty(addonsData))
            {
                HttpContext.Session.SetString("AddonsData", addonsData);
            }
            // Redirigimos al GET para evitar el error 405 y seguir el patrón PRG
            return RedirectToAction(nameof(Index));
        }

        // --- 2. EL GET (Para cargar la vista y procesar los puntos) ---
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Recuperar datos de la sesión (los que guardó el POST y los pasos anteriores)
            var sessionId = HttpContext.Session.GetInt32("SessionId");
            var seatIdsStr = HttpContext.Session.GetString("SelectedSeatIds");
            var seatsSubtotalStr = HttpContext.Session.GetString("SeatsSubtotal");
            var selectedSeatNamesStr = HttpContext.Session.GetString("SelectedSeatNames");

            if (sessionId == null || string.IsNullOrEmpty(seatsSubtotalStr))
            {
                return RedirectToAction("Index", "Home");
            }

            
            //eatsSubtotal = decimal.Parse(seatsSubtotalStr, System.Globalization.CultureInfo.InvariantCulture);
            
            //con esto funciona 
            var seatsSubtotal = decimal.Parse(seatsSubtotalStr, new System.Globalization.CultureInfo("es-ES"));


            var session = await _db.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null) return RedirectToAction("Index", "Home");

            var selectedSeats = selectedSeatNamesStr?.Split(',').ToList() ?? new List<string>();

            // Procesar addons desde la sesión (guardados por el POST)
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
                }).ToList();

            var model = new TicketViewModel
            {
                MovieTitle = session.Movie!.Title,
                RoomNumber = session.Room.RoomId.ToString(),
                SessionTime = session.StartTime.ToString("dd/MM/yyyy HH:mm"),
                SelectedSeats = selectedSeats,
                SeatsSubtotal = seatsSubtotal,
                Addons = selectedAddons,
                Buyer = new BuyerInfo()
            };

            // Lógica del usuario (Igual que la tenías)
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.Buyer.Email = user.Email;
                var employee = await _db.Employees.Include(e => e.BoxOffice).FirstOrDefaultAsync(e => e.Email == user.Email);
                if (employee != null)
                {
                    model.Buyer.BuyerId = employee.EmployeeId;
                    model.Buyer.Name = $"{employee.Firstname} {employee.Lastname}";
                    model.Buyer.Role = "Empleado Staff";
                    model.Buyer.BoxOffice = employee.BoxOffice!.BoxOfficeName;
                    model.Buyer.IsEmployee = true;
                    HttpContext.Session.SetInt32("EmployeeId", employee.EmployeeId);
                }
                else
                {
                    var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);
                    if (customer != null)
                    {
                        model.Buyer.BuyerId = customer.CustomerId;
                        model.Buyer.Name = $"{customer.FirstName} {customer.LastName}";
                        model.Buyer.Role = "Cliente VIP";
                        model.Buyer.IsCustomer = true;
                        model.Buyer.CurrentFidelityPoints = customer.FidelityPoints;
                        model.Buyer.PointsToEarn = selectedSeats.Count * 10;

                        HttpContext.Session.SetInt32("BuyerId", customer.CustomerId);
                        HttpContext.Session.SetInt32("PuntosActuales", customer.FidelityPoints);
                    }
                }
            }
            else
            {
                model.Buyer.Name = "Invitado";
                model.Buyer.Role = "Público General";
            }

            // Recuperar descuento si se aplicó en ApplyPoints
            var descuentoStr = HttpContext.Session.GetString("DescuentoAplicado");
            if (!string.IsNullOrEmpty(descuentoStr))
            {
                model.DescuentoPuntos = decimal.Parse(descuentoStr, System.Globalization.CultureInfo.InvariantCulture);
            }

            return View(model);
        }

        // --- 3. TU MÉTODO APPLY POINTS (Se queda igual, pero ahora el redirect funcionará) ---
        [HttpPost]
        public IActionResult ApplyPoints()
        {
            var puntosActuales = HttpContext.Session.GetInt32("PuntosActuales") ?? 0;
            if (puntosActuales >= 100)
            {
                decimal descuento = 1.00m;
                HttpContext.Session.SetInt32("PuntosACanjear", 100);
                HttpContext.Session.SetString("DescuentoAplicado", descuento.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

