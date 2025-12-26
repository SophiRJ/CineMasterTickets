using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
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
        [ValidateAntiForgeryToken]
        public IActionResult Index(string addonsData)
        {
            Console.WriteLine("LLEGADA POST: " + addonsData);
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
            var addonQuantitiesStr = HttpContext.Session.GetString("AddonsData");

            Console.WriteLine($"DEBUG: Usuario: {User.Identity.Name} - Addons en Sesion: {addonQuantitiesStr}");

            if (sessionId == null || string.IsNullOrEmpty(seatsSubtotalStr))
            {
                return RedirectToAction("Index", "Home");
            }

            //con esto funciona 
            var seatsSubtotal = decimal.Parse(seatsSubtotalStr, new System.Globalization.CultureInfo("es-ES"));


            var session = await _db.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null) return RedirectToAction("Index", "Home");

            var selectedSeats = selectedSeatNamesStr?.Split(',').ToList() ?? new List<string>();

            // Procesar addons desde la sesión (guardados por el POST)

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
                        HttpContext.Session.SetInt32("ActualPoints", customer.FidelityPoints);
                    }
                }
            }
            else
            {
                model.Buyer.Name = "Invitado";
                model.Buyer.Role = "Público General";
            }

            // Recuperar descuento si se aplicó en ApplyPoints
            var descuentoStr = HttpContext.Session.GetString("AppliedDiscount");
            if (!string.IsNullOrEmpty(descuentoStr))
            {
                model.DescuentoPuntos = decimal.Parse(descuentoStr, System.Globalization.CultureInfo.InvariantCulture);
            }

            return View(model);
        }

        // Este metodo hace el calculo para aplicar los puntos del cliente y que gaste todos de golpe.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyPoints()
        {
            // 1. Recuperamos los puntos de la session
            var ActualPoints = HttpContext.Session.GetInt32("ActualPoints") ?? 0;

            // 2. El descuento podrá aplicarse si solo tiene mas de 100 puntos
            if (ActualPoints >= 100)
            {
                // 3. Se va a realizar un descuento simple, en el que los puntos se dividen entre 100
                //y el resultado, se parsea a decimal y sera lo que se descuente. (EJ 150 puntos == 1,50€ desc)
                decimal descuento = ActualPoints / 100m;

                // 4. Se guarda en la sesion el descuento de puntos para meterlo en la vista final
                // y guardamos el total de puntos que se van a restar de la BBDD al final
                HttpContext.Session.SetInt32("PointsToApply", ActualPoints);

                // Guardamos el descuento como string con punto decimal (InvariantCulture)
                HttpContext.Session.SetString("AppliedDiscount", descuento.ToString(System.Globalization.CultureInfo.InvariantCulture));

                // 5. Ponemos los puntos a 0 en la sesión cuando se gasten (solo para la interfaz visual)
                HttpContext.Session.SetInt32("ActualPoints", 0);
            }
            else
            {
                TempData["ErrorPuntos"] = "Necesitas al menos 100 puntos para canjearlos.";
            }

            return RedirectToAction(nameof(Index));
        }
        //Este metodo cancelara los puntos del cliente por si se quiere retractar de usarlos.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelPoints()
        {
            // 1. Recuperamos los puntos que el usuario iba a canjear
            var pointsToApply = HttpContext.Session.GetInt32("PointsToApply") ?? 0;

            if (pointsToApply > 0)
            {
                // 2. Devolvemos esos puntos al contador visual de la sesión
                HttpContext.Session.SetInt32("ActualPoints", pointsToApply);

                // 3. Borramos los datos del descuento de la sesión
                HttpContext.Session.Remove("PointsToApply");
                HttpContext.Session.Remove("AppliedDiscount");
                //Mensjae informativo para que el cliente sepa que se han recuperado los puntos
                TempData["Message"] = "Descuento cancelado y puntos restaurados.";
            }

            return RedirectToAction(nameof(Index));
        }

        //Metodo simple para calcular el metodo de pago (este solo sirve para que el usuario seleccione el metodo de pago:
        //Si es invitado o Cliente solo podra ser por tarjeta
        //Si es Empleado, podra elegir entre efectivo o tarjeta
        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            // 1. Verificamos el Rol del usuario para la vista
            var user = await _userManager.GetUserAsync(User);
            bool isEmployee = false;

            if (user != null)
            {
                // Comprobamos si el email existe en la tabla de empleados
                isEmployee = user != null && await _db.Employees.AnyAsync(e => e.Email == user.Email);
            }

            // 2. Pasamos esta info a la vista para filtrar los botones segun el rol que tenga
            ViewBag.IsEmployee = isEmployee;

            return View();
        }

        //El metodo mas importante: Este metodo muestra el ticket final de compra, y lo guarda en la BBDD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPurchase(string paymentMethod)
        {
            // 1. Recuperar datos de sesión necesarios para mostrar y guardarlos en la BBDD
            var sessionId = HttpContext.Session.GetInt32("SessionId");
            var seatIdsStr = HttpContext.Session.GetString("SelectedSeatIds");
            var addonQuantitiesStr = HttpContext.Session.GetString("AddonsData");
            var seatsSubtotalStr = HttpContext.Session.GetString("SeatsSubtotal");
            var descuentoStr = HttpContext.Session.GetString("DescuentoAplicado");
            var buyerId = HttpContext.Session.GetInt32("BuyerId");
            var employeeId = HttpContext.Session.GetInt32("EmployeeId");

            //Control para comprobar si hay sesion o asientos, si no devuelve a la pagina del ticket
            if (sessionId == null || string.IsNullOrEmpty(seatsSubtotalStr) || string.IsNullOrEmpty(seatIdsStr))
                return RedirectToAction("Index");

            //Aqui se prepara en una variable los id de los asientos
            var seatIds = seatIdsStr.Split(',').Select(int.Parse).ToList();

            // Empezamos a recopilar datos para calcular el precio total (descuento, precio de los asientos...etc)
            decimal subtotalAsientos = decimal.Parse(seatsSubtotalStr, new System.Globalization.CultureInfo("es-ES"));
            decimal descuento = !string.IsNullOrEmpty(descuentoStr)
                ? decimal.Parse(descuentoStr, System.Globalization.CultureInfo.InvariantCulture) : 0;

            //Inicializamos la variable que contendra el total de los addons si hay a 0
            decimal subtotalAddons = 0;
            //Creamos un diccionario para recoger los valores del JSON de los Complementos(recordemos que los
            //hemos guardado en un JSON como 5:1, donde 5 es el id y 1 la cantidad
            var addonQuantities = new Dictionary<int, int>();
            if (!string.IsNullOrEmpty(addonQuantitiesStr))
            {
                addonQuantities = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(addonQuantitiesStr)!;
                foreach (var item in addonQuantities)
                {
                    //Recogemos la key del Complemento de la base de datos para saber de cual se trata
                    var addon = await _db.AddOns.FindAsync(item.Key);
                    //Hacemos el calculo del precio total multiplicando su precio por su cantidad
                    if (addon != null) subtotalAddons += (addon.Price * item.Value);
                }
            }
            //Por fin, calculamos el precio total
            decimal totalCalculado = (subtotalAsientos + subtotalAddons) - descuento;

            // 2. Preparamos el objeto Ticket
            var nuevoTicket = new Ticket
            {
                SessionId = sessionId.Value,
                CustomerId = buyerId,
                EmployeeId = employeeId,
                //Tenemos que recuperar el tipo de pago a traves de la propiedad del enumerado
                PaymentMethod = (PaymentMethodType)Enum.Parse(typeof(PaymentMethodType), paymentMethod),
                //La fecha sera la del momento exacto de compra
                PurchasedAt = DateTime.Now,
                //Si ha sigo un empleado, recuperaremos el valor de la taquilla donde lo ha comprado
                SoldAtBoxOffice = employeeId.HasValue,
                EmailToSend = User.Identity?.Name,
                TotalPrice = totalCalculado
            };

            // 3. Guardar los datos correspondientes a los asientos en la tabla que relaciona los asientos 
            //con el ticket para que aparezcan reflejados en el ticket, y en la que relaciona los asientos
            //con la sesion para que aparezcan bloqueados y no los pueda coger nadie mas.
            foreach (var sId in seatIds)
            {
                nuevoTicket.TicketSeats.Add(new TicketSeat
                {
                    SeatId = sId,
                    SessionId = sessionId.Value
                });

                _db.SessionSeats.Add(new SessionSeat
                {
                    SessionId = sessionId.Value,
                    SeatId = sId
                });
            }

            // 4. Guardar Complementos para que queden reflejados en el ticket con su tabla de relacion correspondiente
            foreach (var item in addonQuantities)
            {
                nuevoTicket.TicketAddOns.Add(new TicketAddOn
                {
                    //Aqui solo tenemos que añadir el id del Complemento o complementos que el usuario haya escogido
                    AddOnId = item.Key
                    
                });
            }

            // 5. Si el usuario es Customer, aplicamos la logica de puntos
            if (buyerId.HasValue)
            {
                //Buscamos primero en la base de datos su id. Devemos acceder a traves de su Value
                //ya que el valor de este puede ser null (?) (no seria un int), si no da error de compilacion
                var customer = await _db.Customers.FindAsync(buyerId.Value);
                if (customer != null)
                {
                    //Recuperamos los puntos para canjear
                    int puntosACanjear = HttpContext.Session.GetInt32("PuntosACanjear") ?? 0;
                    //Calculamos la logica de puntos que va a ganar el usuario por la compra actual
                    int puntosGanados = seatIds.Count * 50;
                    //Actualizamos los puntos del usuario en su base de datos
                    customer.FidelityPoints = (customer.FidelityPoints - puntosACanjear) + puntosGanados;
                    _db.Update(customer);
                }
            }

            // 6. Guardado final
            //Una vez que ya hemos construido todo el conjunto del ticket junto con sus relaciones u objetos
            //asociados, pasamos a guardar el objeto ticket total
            _db.Tickets.Add(nuevoTicket);
            await _db.SaveChangesAsync();

            //Y guardamos el Id del ticket en la sesion junto con el metodo de pago final
            HttpContext.Session.SetInt32("LastTicketId", nuevoTicket.TicketId);
            HttpContext.Session.SetString("MetodoPagoFinal", paymentMethod);

            return RedirectToAction(nameof(FinalTicket));
        }

        //Metodo para mostrar el ticket final cuando ya se ha realizado la compra
        [HttpGet]
        public async Task<IActionResult> FinalTicket()
        {
            // 1. Recuperamos de nuevo todos los datos de la sesion para mostrarlos en el ticket
            var ticketId = HttpContext.Session.GetInt32("LastTicketId");
            var metodoPago = HttpContext.Session.GetString("MetodoPagoFinal");
            var sessionId = HttpContext.Session.GetInt32("SessionId");

            if (ticketId == null || sessionId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var seatNames = HttpContext.Session.GetString("SelectedSeatNames") ?? "";
            var seatsSubtotalStr = HttpContext.Session.GetString("SeatsSubtotal");
            var addonQuantitiesStr = HttpContext.Session.GetString("AddonsData");
            var descuentoStr = HttpContext.Session.GetString("DescuentoAplicado");

            // 2. Obtener datos de la película y sala (necesario para el nombre de la película)
            var session = await _db.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            // 4. Construir el ViewModel para la vista igual que en los metodos de antes
            decimal seatsSubtotal = decimal.Parse(seatsSubtotalStr ?? "0", new System.Globalization.CultureInfo("es-ES"));
            decimal descuento = !string.IsNullOrEmpty(descuentoStr)
                ? decimal.Parse(descuentoStr, System.Globalization.CultureInfo.InvariantCulture) : 0;

            var addonQuantities = string.IsNullOrEmpty(addonQuantitiesStr)
                ? new Dictionary<int, int>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(addonQuantitiesStr)!;

            var allAddons = await _db.AddOns.ToListAsync();
            var selectedAddons = allAddons
                .Where(a => addonQuantities.ContainsKey(a.AddOnId))
                .Select(a => new AddonItem
                {
                    Name = a.AddOnName,
                    UnitPrice = a.Price,
                    Quantity = addonQuantities[a.AddOnId]
                }).ToList();


            var model = new TicketViewModel
            {
                MovieTitle = session?.Movie?.Title ?? "Película",
                RoomNumber = session?.Room?.RoomId.ToString() ?? "-",
                SessionTime = session?.StartTime.ToString("dd/MM/yyyy HH:mm") ?? "-",
                SelectedSeats = seatNames.Split(',').ToList(),
                SeatsSubtotal = seatsSubtotal,
                Addons = selectedAddons,
                DescuentoPuntos = descuento,
                Buyer = new BuyerInfo { Name = User.Identity?.Name ?? "Cliente" }
            };

            // Pasamos los datos extra por ViewBag
            ViewBag.TicketId = ticketId;
            ViewBag.MetodoPago = metodoPago;
            ViewBag.FechaCompra = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            return View(model);
        }
    }
}

