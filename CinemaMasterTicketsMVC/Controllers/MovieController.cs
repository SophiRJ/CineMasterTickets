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
    //Este metodo carga los datos totales de una pelicula en concreto (por su id):
    //- Sus sesiones
    //- Sus dias
    //- Sus datos (cartel, generos, director, duracion, sinopsis...)
    //Se ha decidido hacer mediante includes, ya que en el resto del código se han utilizado ViewModels
    //para adaptar los datos a las vistas. Así se demuestran ambas formas de hacerlo.
    public async Task<IActionResult> Index(int id)
    {
        HttpContext.Session.SetInt32("MovieId", id);

        //Se cargan los datos correspondientes a la pelicula necesarios para esta vista
        var movie = await _db.Movies
            .Include(m => m.Sessions.Where(s => s.Status == "Active" && s.StartTime > DateTime.Now))
                .ThenInclude(s => s.Tickets)
                    .ThenInclude(t => t.TicketSeats)
            .Include(m => m.Sessions)
                .ThenInclude(s => s.Room)
                    .ThenInclude(r => r!.Rows)
                        .ThenInclude(row => row.Seats)
            .FirstOrDefaultAsync(m => m.MovieId == id);

        //Manejo de errores
        if (movie == null)
            return NotFound();

        // Agrupamos sesiones por día
        var sessionsByDay = movie.Sessions.Where(s => s.MovieId == id && s.Status == "Active"
            && s.StartTime > DateTime.Now)
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
                    //retornamos un objeto nuevo con los datos completos de la sesion y un calculo
                    //que directamente comprueba que queden asientos disponibles en la sesion para 
                    //no tener que calcularlo en la vista o mas adelante
                    return new
                    {
                        Session = s,
                        HasAvailableSeats = reservedSeats < totalSeats
                    };
                }).ToList()
            })
            .ToList();
        //Le pasamos el calculo de las sesiones con la comprobacion de si estan disponibles o no para 
        //Que se use en la vista (botones)
        ViewBag.SessionsByDay = sessionsByDay;

        return View(movie);
    }
    //Metodo para retornar hacia la vista de Home/Index. Normalmente no seria necesario,
    //pero se ha considerado implementarlo para borrar los datos de la session por buena practica.
    public IActionResult BackToHome()
    {
        // Borramos TODOS los datos de la session para evitar errores
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    //Este metodo recupera , a traves del id de la sesion, los datos necesarios para mostrar los 
    //datos de la pelicula y para que el Js pueda representar junto con la vista los asientos
    //que le pertenecen a esa sala.
    public async Task<IActionResult> SeatSelection(int sessionId)
    {
        //Guardamos el Id de la sesion en el SessionData para ya tenerlo disponible despues
        HttpContext.Session.SetInt32("SessionId",sessionId);
        //Recuperamos en una lista los Ids de los asientos ya reservados en esa sala para esa sesion.
        var reservedSeatIds = await _db.TicketSeats
        .Where(ts => ts.SessionId == sessionId)
        .Select(ts => ts.SeatId)
        .ToListAsync();
        // Recuperamos de la sesión una cadena de texto que contiene los IDs de asientos 
        // que el usuario ha ido seleccionando.
        var selectedSeatIdsStr = HttpContext.Session.GetString("SelectedSeatIds");
        //Si la cadena de texto no esta vacia, convertimos cada ID de asiento de tipo Str a int y lo guardamos en una lista
        var selectedSeatIds = string.IsNullOrEmpty(selectedSeatIdsStr)
            ? new List<int>()
            : selectedSeatIdsStr.Split(',').Select(int.Parse).ToList();

        //Creamos el ViewModel que contendrá los datos necesarios para esta vista
        var vm = await _db.Sessions
            .Where(s => s.SessionId == sessionId)
            .Select(s => new SeatSelectionViewModel
            {
                SessionId = s.SessionId,
                MovieTitle = s.Movie!.Title!,
                StartTime = s.StartTime,
                Price = s.Price,
                RoomId = s.RoomId,
                // Aquí se genera el mapa de butacas de la sala:
                SeatMap = s.Room!.Rows
                //Ordenamos las filas alfabéticamente (A,B,C...)
                    .OrderBy(r => r.Name)
                    .Select(row => new RowSeats
                    {
                        //Asignamos un nombre a la fila
                        Row = row.Name.ToString(),
                        //Le adjuntamos un conjunto de asientos
                        Seats = row.Seats
                            .OrderBy(seat => Convert.ToInt32(seat.Number)) //Ordenandolos por numero
                            .Select(seat => new SeatInfo
                            {
                                //Cada asiento contendra un id, un numero y la info de si esta reservado o selecionado
                                SeatId = seat.SeatId,
                                Number = seat.Number,
                                //Estas dos variables las utilizamos para representar los asientos reservados
                                //o seleccionados por primera vez cuando se entra a la pagina. Si la lista
                                //de numeros contiene los ids, la vista los pintara del color correspondiente.
                                IsReserved = reservedSeatIds.Contains(seat.SeatId),
                                IsSelected = selectedSeatIds.Contains(seat.SeatId)
                            }).ToList()
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();
        //Guardamos el titulo de la pelicula y la sesion para utilizarlos posteriormente
        HttpContext.Session.SetString("MovieTitle", vm.MovieTitle);
        HttpContext.Session.SetInt32("RoomId", vm.RoomId);
        HttpContext.Session.SetString("SessionTime", vm.StartTime.ToString("dd/MM/yyyy HH:mm"));

        //Devolvemos el viewModel a la vista para representarlo
        return View(vm);
    }

    //Este metodo retorna desde la seleccion de asientos hacia la seleccion de sesion.
    //Para ello, vamos a ir restando gradualmente los datos necesarios de la session
    public IActionResult CancelPurchase()
    {
        //Volvemos a recoger la id de la sesion para poder entrar al metodo index ya que necesita como
        //parametro el id de la pelicula de la que va a mostrare sus datos.
        var movieId = HttpContext.Session.GetInt32("MovieId");
        //Borramos de la Session el id de la sesion y los asientos seleccionados si hubiera alguno
        HttpContext.Session.Remove("SessionId");
        HttpContext.Session.Remove("SelectedSeatIds");
        //Si se ha podido recuperar el id de la pelicula, redireccionamos amovie/index, si no, directamente al
        //inicio de la aplicacion.
        if (movieId.HasValue)
        {
            return RedirectToAction("Index", "Movie", new { id = movieId.Value });
        }
        return RedirectToAction("Index", "Home");
    }

    //Metodo POST que guarda Los asientos seleccionados en la SessionData y redirige a la vista de seleccion
    //de complementos. Para guardar los datos utilizamos un ViewModel.
    [HttpPost]
    public IActionResult SeatSelectionPost(SeatSelectionViewModel model)
    {

        //Si la variableque guarda los asientos seleccionados esta vacía (no se ha seleccionado ningún asiento)
        //se manda un mensaje a la vista y se recarga la pagina de seleccion de asientos recreando el ID de la sesion
        if (string.IsNullOrEmpty(model.SelectedSeats))
        {
            TempData["ErrorMessage"] = "Debes seleccionar al menos un asiento.";
            return RedirectToAction("SeatSelection", new { sessionId = model.SessionId });
        }
        //Si todo va bien, vamos guardando los datos del ViewModel a la SessionData para poder utilizarlos despues
        HttpContext.Session.SetString("SelectedSeatIds", model.SelectedSeats);
        HttpContext.Session.SetString("SelectedSeatNames", model.SelectedSeatNames ?? "");
        HttpContext.Session.SetString("SeatUserTypes", model.SeatUserTypes ?? "");
        HttpContext.Session.SetString("SeatsSubtotal",model.TotalSeatsPrice!);
        //Y redirije a la vista de seleccion de AddOns (Complementos)
        return RedirectToAction("AddOns");
    }

    //Metodo para mostrar la vista de Addons
    [HttpGet]
    public IActionResult AddOns()
    {
        //Recuperamos la sesion y el precio del total de asientos
        var sessionId = HttpContext.Session.GetInt32("SessionId");
        var seatsPriceStr = HttpContext.Session.GetString("SeatsSubtotal");
        var roomId = HttpContext.Session.GetInt32("RoomId");

        if (sessionId == null || string.IsNullOrEmpty(seatsPriceStr))
        {
            return RedirectToAction("Index", "Home");
        }

        // Convertimos el string de la sesión a decimal para poder recalcular los precios cuando se vayan
        //añadiendo complementos
        decimal seatsPrice = decimal.Parse(seatsPriceStr, new CultureInfo("es-ES"));
        //Creamos un ViewModel simplemente con el Selector de tipos de los AddOns
        var vm = new AddOnsViewModel
        {
            AddOnTypesSelectList = new SelectList(Enum.GetValues(typeof(AddOnType)).Cast<AddOnType>())
        };

        // En este metodo pasamos los datos simplemente con ViewBags para que puedan ser utilizados en la vista
        ViewBag.SeatsSubtotal = seatsPrice; 
        ViewBag.SeatsSubtotalRaw = seatsPrice.ToString(System.Globalization.CultureInfo.InvariantCulture); // Para el JS
        //El resto de ViewBags los inicializamos con los datos correspondientes de la Session
        ViewBag.MovieTitle = HttpContext.Session.GetString("MovieTitle") ?? "Película";
        ViewBag.SessionTime = HttpContext.Session.GetString("SessionTime") ?? "";
        ViewBag.SessionId = sessionId;
        ViewBag.RoomId = roomId;
        //Pasamos el ViewModel para el selector
        return View(vm);
    }

    //Este método sirve para devolver los addons correspondientes por tipo y devolverlos en JSON para que el
    //Js correspondiente los pueda representar dinamicamente.
    [HttpGet]
    public async Task<IActionResult> GetAddOnsByType(string type)
    {
        // Obtenemos todos los addons activos
        var addons = await _db.AddOns.Where(a => a.IsActive).ToListAsync();

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
        //Devolvemos el JSON
        return Json(result);
    }

    //Este metodo se encarga de guardar los datos de los complementos seleccionados en la Session y 
    //redirigir a la vista Index del controlador de Ticket para pasar a la fase final
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmAddons(string addonsData)
    {
        //Si el string tiene datos, lo guardamos en la Session
        if (!string.IsNullOrEmpty(addonsData))
            {
                HttpContext.Session.SetString("AddonsData", addonsData);
            }
        //Redirigimos al Index de Ticket
            return RedirectToAction("Index", "Ticket");
    }
}

