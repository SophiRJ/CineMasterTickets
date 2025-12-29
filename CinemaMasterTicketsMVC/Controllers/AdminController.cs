using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
using CinemaMasterTicketsMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Reflection;
using X.PagedList;
using X.PagedList.Extensions;

namespace CinemaMasterTicketsMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole>? _roleManager;
        private readonly UserManager<IdentityUser>? _userManager;
        //Agregamos la proviedad privada _configuration, la cual extiende de la interfaz IConfiguration
        //para poder acceder a las variables de entorno y extraer la APIKey
        private readonly IConfiguration _configuration;
        public ApplicationDbContext _db;


        public AdminController(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager, IConfiguration configuration,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _configuration = configuration;
        }
        //Este Action lleva al panel de control del Admin
        public IActionResult Home()
        {
            return View();
        }

        //Metodo que trae los empleados y los usuarios ACTIVOS
        public async Task<IActionResult> Index()
        {
            var model = new AdminUsersPanelViewModel
            {

                Employees = await _db.Employees
        .Include(e => e.BoxOffice)
        .Where(e => e.isActive) // <-- Filtro fundamental para el borrado lógico
        .OrderBy(e => e.Lastname)
        .ToListAsync(),

                // Traemos clientes
                Customers = await _db.Customers
                .Where(c => c.isActive)
                .OrderBy(c => c.LastName)
                .ToListAsync()
                };

            return View(model);
        }

//Metodo para crear un empleado
        public IActionResult CreateEmployee()
        {
            //Creamos el ViewModel con la lista de Lugares de trabajo y se la mandamos a la vista.
            var model = new CreateEmployeeBoxOficceViewModel
            {
                BoxOffices = new SelectList(_db.BoxOffices, "BoxOfficeId", "BoxOfficeName")
            };

            return View(model);
        }

        //Metodo para guardar un empleado creado que recibe el VM correspondiente con los datos rellenos
        //para procesarlo en BBDD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeBoxOficceViewModel vm)
        {
            //Si el modelo no es valido, reconstruimos en select y redirigimos a la vista otra vez
            if (!ModelState.IsValid)
            {
                vm.BoxOffices = new SelectList(_db.BoxOffices, "BoxOfficeId", "BoxOfficeName");

                return View(vm);
            }

            //IMPORTANTE: Generamos una contraseña automatica, la cual será el apellido del empleado + 123!
            //Esto solo ocurre con los perfiles de EMPLEADO
            var pass = vm.Employee!.Lastname + "123!";
            //Lo guardamos en la tabla Users de identity
            var user = new IdentityUser
            {
                UserName = vm.Employee.Email,
                Email = vm.Employee.Email,
                EmailConfirmed = true
            };

            var result = await _userManager!.CreateAsync(user, pass);

            //Si hay un error, mandamos el error
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(vm);
            }

            //Asignamos el rol de empleado
            await _userManager.AddToRoleAsync(user, "Employee");
            //y lo guardamos en BBDD (nuestra tabla) en empleados
            _db.Employees.Add(vm.Employee);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

       //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployeeAjax(int id)
        {
            // 1. Buscar al empleado incluyendo sus relaciones si fuera necesario
            var employee = await _db.Employees.FindAsync(id);

            if (employee == null)
                return Json(new { success = false, message = "Empleado no encontrado." });

            try
            {
                // 2. Borrar el usuario de Identity (Seguridad)
                // Buscamos por Email para eliminar su cuenta de acceso permanentemente
                var user = await _userManager!.FindByEmailAsync(employee.Email!);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        return Json(new { success = false, message = "Error al eliminar las credenciales de acceso." });
                    }
                }

                // 3. Borrado Lógico en la tabla de Empleados
                // En lugar de _db.Employees.Remove(employee), cambiamos su estado
                employee.isActive = false; // Asumiendo que tu modelo Employee tiene IsActive (si es isActive, cámbialo a minúscula)

                _db.Employees.Update(employee);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Empleado desactivado y acceso eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno: " + ex.Message });
            }
        }
        
        //Este metodo se encarga de mostrar el listado de peliculas disponibles desde la API externa 
        //de TheMovieDB
        public async Task<IActionResult> SelectFilmsAPI(int? pageNumber, string searchString)
        {
            //Variables necesarias para el paginador, apiKey y el buscador
            int page = pageNumber ?? 1;
            //La APIKey se recoge desde las variables de entorno
            string apiKey = _configuration["MovieApi:ApiKey"] ?? ""; 
            int pageSize = 10;

            ViewBag.CurrentSearch = searchString;

            //Aqui, dependiendo de si hay busqueda o no construimos la ruta correspondiente
            string url;
            if (string.IsNullOrEmpty(searchString))
            {
                url = $"https://api.themoviedb.org/3/movie/popular?api_key={apiKey}&language=es-ES&page={page}";
            }
            else
            {
                url = $"https://api.themoviedb.org/3/search/movie?api_key={apiKey}&language=es-ES&query={Uri.EscapeDataString(searchString)}&page={page}";
            }
            //Es necessario crear un cliente Http para poder conectar con el API ya que si no tendriamos
            //que meter todos lso datos a mano o usar librerias externas

            //Valorar si integrarlo en el program si vamos a hacer muchos Clients
            using (var httpClient = new HttpClient())
            {
                //Conectamos con la Url de las dos opciones anteriores(la que haya usado)
                var response = await httpClient.GetAsync(url);

                //Si el status de la conexion es favorable
                if (response.IsSuccessStatusCode)
                {
                    //Hay que traducir la respuesta enviada para poder pasarlas a string(las pelis de dentro de la respuesta
                    var jsonString = await response.Content.ReadAsStringAsync();
                    //Usamos DeserializeObject para transformar el string a Json y se guarda en un tipo dinamico
                    dynamic apiData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString)!;

                    //Hacemos una lista de las pelis con el resultado anterior para poder manejarla en la View
                    var movies = ((IEnumerable<dynamic>)apiData!.results).Take(pageSize).ToList();

                    //El movimiento siguiente es interesante:
                    //Por cada pelicula se van pidiendo los detalles necesarios que no encontramos en la llamada
                    //de la URL normal (hay que hacer una segunda llamada para acceder a datos como el director o la 
                    //duracion de la pelicula).

                    // Obtenemos los IDs de las películas que YA están en nuestra base de datos
                    var existingMovieIds = await _db.Movies
                        .Select(m => m.MovieAPIId)
                        .ToListAsync();

                    foreach (var movie in movies)
                    {
                        try
                        {
                            //Creamos esta variable isAdded, donde vamos a contener una pequeña consulta a la lista de Ids de 
                            //la API, para saber si contiene el id o no (lo usaremos en la vista como bool)
                            bool isAdded = existingMovieIds.Contains(movie.id.ToString());
                            //Se lo pasamos al listado dinamico para utilizarlo en la vista (se guarda como JValue, habra que castearlo
                            //en la vista si no queremos que de fallo de lectura.
                            movie.IsAlreadyInDb = isAdded;
                            //Llamada a la url para los detalles de la pelicula con GetAsync
                            string detailUrl = $"https://api.themoviedb.org/3/movie/{movie.id}?api_key={apiKey}&language=es-ES&append_to_response=credits";
                            var detailResponse = await httpClient.GetAsync(detailUrl);

                            //Si la llamada tiene exito...
                            if (detailResponse.IsSuccessStatusCode)
                            {
                                //Recogemos el contenido de la respuesta que vuelve en JSON y lo deserializamos
                                var detailJson = await detailResponse.Content.ReadAsStringAsync();
                                dynamic detailData = Newtonsoft.Json.JsonConvert.DeserializeObject(detailJson)!;
                                //Recogemos la duracion con un ternario por si viene vacío
                                movie.runtime = detailData!.runtime != null ? (int)detailData.runtime : 0;
                                //Tambien capturamos la seccion donde se define el director y lo inicializamos a "Desconocido"
                                var crew = detailData.credits.crew;
                                string directorName = "Desconocido";

                                //Si hay director...
                                if (crew != null)
                                {
                                    foreach (var dir in crew)
                                    {
                                        //Si en los datos (dri.job) coincide alguno con el Director, capturamos su nombre
                                        if (dir.job == "Director")
                                        {
                                            directorName = dir.name;
                                            break; 
                                        }
                                    }
                                }
                                //Lo guardamos en el objeto
                                movie.director = directorName;
                            }
                            else
                            {
                                movie.runtime = 0;
                            }
                        }
                        catch
                        //Si falla alguna llamada, directamente ponemos la duración a 0 para evitar errores
                        {
                            movie.runtime = 0;
                        }
                    }
                    //Aqui guardamos el total de pelis existentes en esta busqueda
                    int totalResults = (int)apiData.total_results;

                    //Es necesario poner un limite de pelis para que no reviente la API (solo deja hasta 500)
                    int maxAllowedResults = 500 * pageSize;
                    //Esto garantiza que el paginador nunca intente crear un botón para la página 501,
                    //evitando que la aplicacion falle.
                    int finalCount = Math.Min(totalResults, maxAllowedResults);

                    // Aqui es necesario usar StaticPagedList ya que al no poseer una lista procedente de una BBDD es necesario.
                    var model = new StaticPagedList<dynamic>(movies, page, pageSize, finalCount);

                    return View(model);
                }
            }

            // En caso de error o si la API falla, devolvemos una lista paginada vacía, para que no rompa la vista
            return View(new StaticPagedList<dynamic>(new List<dynamic>(), 1, pageSize, 0));
        }


        //Metodo para agregar películas seleccionadas por el administrador a la base de datos. Las captura desde el JS con AJAX
        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] Movie movie)
        {
            //Captura de errores
            if (movie == null) return BadRequest("Datos inválidos");
            //Le damos valor a la fecha de añadido a la BBDD con fecha actual
            movie.AddedAt = DateTime.Now;
            //Añadimos la pelicula a la BD
            _db.Add(movie);
            await _db.SaveChangesAsync();
            //Retornamos un mensaej de exito
            return Ok(new { message = "Película añadida correctamente" });
        }

        //Metodo que muestra todos los Addons
        //Se ha modificado por un error a ultima hora. Se pasa con ViewBags por simpleza
        public async Task<IActionResult> GetAddons()
        {
            //Obtenemos todos los addons con sus tickets
            var allAddons = await _db.AddOns
                .Include(a => a.TicketAddOns)
                .ToListAsync();
            //Creamos tres viewBags para paser la informacion a la vista de si estan activos,
            //inactivos o si tienen ventas a traves de consultas
            ViewBag.Active = allAddons.Where(a => a.IsActive).ToList();
            ViewBag.Inactive = allAddons.Where(a => !a.IsActive).ToList();
            ViewBag.IdsWithSales = allAddons.Where(a => a.TicketAddOns.Any())
                                            .Select(a => a.AddOnId)
                                            .ToList();

            return View();
        }

        //Este metodo POST se encarga de cambiar los estados de los AddOns de Activo a inactivo
        //a traves del id del AddOn selecionado
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> ToggleStatus(int id)
        {
            //Buscamos el addOn por su ID
            var addon = await _db.AddOns.FindAsync(id);
            //Si no lo encontramos devolvemos un mensaje de error por JSON
            if (addon == null) return Json(new { success = false, message = "AddOn no encontrado" });

            //Bandera para cambiar el estado a su forma contraria
            addon.IsActive = !addon.IsActive;
            _db.Update(addon); // Aseguramos que EF marque el cambio en la BBDD
            await _db.SaveChangesAsync();
            //Retornamos el JSON al JS para que maneje el objeto y lo coloque en la tabla correspondiente
            return Json(new { success = true, newState = addon.IsActive });
        }


        //Este metodo sirve para crear nuevos AddOns. Simplemente pasa un selectList con los tipos
        //de addOns disponibles para que el usuario pueda escoger uno al crearlo.
        public IActionResult AddAddon()
        {
            ViewBag.AddOnTypes = Enum.GetValues(typeof(AddOnType))
                                     .Cast<AddOnType>()
                                     .Select(a => new SelectListItem
                                     {
                                         Value = a.ToString(),
                                         Text = a.ToString()
                                     }).ToList();

            return View();
        }

        //Metodo para guardar un AddOn creado en la pagina de crear Addon. Recibe un 
        //objeto Addon con sus caracetrísticas listas para guardar.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddon(AddOn addOn)
        {
            // Preparamos dropdown nuevamente en caso de error
            ViewBag.AddOnTypes = Enum.GetValues(typeof(AddOnType))
                                     .Cast<AddOnType>()
                                     .Select(a => new SelectListItem
                                     {
                                         Value = a.ToString(),
                                         Text = a.ToString()
                                     }).ToList();
            //Hacemos una comprobacion en la BBDD para saber si el addon existe ya con ese nombre
            bool existeNombre = _db.AddOns.Any(a => a.AddOnName!.ToLower() == addOn.AddOnName!.ToLower());
            //Si existe, mostramos un error y se lo comunicamos al usuario
            if (existeNombre)
            {
                ModelState.AddModelError("AddOnName", "Ya existe un AddOn con este nombre.");
            }
            //Si el modelo está bien formado
            if (ModelState.IsValid)
            {
                //pasamos a guardar la imagen, pero primero hay que construir su "nombre" o "ruta"
                if (addOn.AddOnImageFile != null && addOn.AddOnImageFile.Length > 0)
                {
                    // Extraemos su extension,generamos un nombre único(para que no coincidan al crearse)
                    //y creamos la ruta donde se va a guardar en la base de datos
                    var extension = Path.GetExtension(addOn.AddOnImageFile.FileName);
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/addons", fileName);

                    // Guardamos el archivo en wwwroot/img/addons
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await addOn.AddOnImageFile.CopyToAsync(stream);
                    }

                    // Guardamos la ruta relativa en la base de datos
                    addOn.AddOnImage = $"img/addons/{fileName}";
                }

                // Guardamos ya el objeto en la base de datos
                _db.AddOns.Add(addOn);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(GetAddons));

            }
            //Si algo sale mal, retornamos a la misma vista devolviendo el objeto apra que lo muestre
            return View(addOn);
        }

        //Action para editar un AddOn por su ID
        public async Task<IActionResult> EditAddOn(int id)
        {
            //Lo buscamos en la BBDD por el ID
            var addOn = await _db.AddOns.FindAsync(id);
            if (addOn == null)
                return NotFound();
            //Creamos un Select para poder elegir el tipo y lo pasamos por ViewBag
            ViewBag.AddOnTypes = Enum.GetValues(typeof(AddOnType))
                .Cast<AddOnType>()
                .Select(a => new SelectListItem
                {
                    Value = a.ToString(),
                    Text = a.ToString()
                }).ToList();

            return View(addOn);
        }

        //Action para guardar el AddOn editado (parametros-> Id y Objeto AddOn)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddOn(int id, AddOn addOn)
        {
            if (id != addOn.AddOnId)
                return BadRequest();

            ViewBag.AddOnTypes = Enum.GetValues(typeof(AddOnType))
                .Cast<AddOnType>()
                .Select(a => new SelectListItem
                {
                    Value = a.ToString(),
                    Text = a.ToString()
                }).ToList();

            // Comprobamos si el nombre esta ya en la BBDD para que no haya duplicados
            bool existeNombre = _db.AddOns.Any(a => a.AddOnName!.ToLower() == addOn.AddOnName!.ToLower() && a.AddOnId != id);
            if (existeNombre)
            {
                ModelState.AddModelError("AddOnName", "Otro producto ya utiliza este nombre.");
            }
            //Si el modelo no esta bien formado, lo retornamos a la vista
            if (!ModelState.IsValid)
                return View(addOn);
            //Comprobamos tambien que el objeto a modificar exista en la BBDD
            var addOnDb = await _db.AddOns.FindAsync(id);
            if (addOnDb == null)
                return NotFound();

            // Actualizamos los campos del de la BBDD con los del parametro
            addOnDb.AddOnName = addOn.AddOnName;
            addOnDb.Price = addOn.Price;
            addOnDb.Type = addOn.Type;

            // Si se sube una nueva imagen
            if (addOn.AddOnImageFile != null && addOn.AddOnImageFile.Length > 0)
            {

                //Borramos la imagen antigua de la carpeta
                if (!string.IsNullOrEmpty(addOnDb.AddOnImage))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", addOnDb.AddOnImage);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }


                // Volvemos a utilizar la logica de creacion del Path y el nombre de la imagen
                var extension = Path.GetExtension(addOn.AddOnImageFile.FileName);
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/addons", fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await addOn.AddOnImageFile.CopyToAsync(stream);

                addOnDb.AddOnImage = $"img/addons/{fileName}";
            }
            //Guardamos en la base de datos
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(GetAddons));
        }

        //Metodo paa borrar un Complemento (AddOn)
        public async Task<IActionResult> DeleteAddOn(int id)
        {
            // Usamos Include para traer la colección de tickets
            var addOn = await _db.AddOns
                .Include(a => a.TicketAddOns)
                .FirstOrDefaultAsync(m => m.AddOnId == id);

            if (addOn == null)
                return NotFound();
            //Pasamos por ViewBag las comprobaciones de si tiene ventas y cuantas ventas tiene
            ViewBag.TieneVentas = addOn.TicketAddOns.Any();
            ViewBag.TotalVentas = addOn.TicketAddOns.Count;

            return View(addOn);
        }

        //Metodo para borrar un AddOn QUE NO TENGA VENTAS por ID
        //INTERESANTE: ActionName nos ha permitido renombrar el metodo para que los botones de la vista
        //nos permitan llamarlo de otra forma mas entendible y asi poder distinguir metodos correctamente
        [HttpPost, ActionName("DeleteAddOn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Traemos la lista de AddOns
            var addOn = await _db.AddOns
                .Include(a => a.TicketAddOns)
                .FirstOrDefaultAsync(m => m.AddOnId == id);

            if (addOn == null) return NotFound();

            
            if (addOn.TicketAddOns.Any())
            {
                // Si justo alguien compró uno, detenemos el borrado y mandamos error a la vista
                ModelState.AddModelError("", "No se puede eliminar: este AddOn acaba de registrar una venta.");
                return View(addOn);
            }

            // Si no tiene tickets, procedemos al borrado físico
            if (!string.IsNullOrEmpty(addOn.AddOnImage))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", addOn.AddOnImage);
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _db.AddOns.Remove(addOn);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(GetAddons));
        }


        //Action para mostrar datos de ventas, el cual recibe los parametros necesarios para las búsquedas y paginación
        public async Task<IActionResult> SalesReport(int? searchId, DateTime? searchDate, decimal? searchPrice, string? searchUser, bool? searchBoxOffice, int? page)
        {
            //Tamaño de pagina y numeración de ellas
            int pageSize = 3;
            int pageNumber = page ?? 1;

            //Creamos un nuevo ViewModel con los parametros recogidos en la búsqueda para la vista
            var viewModel = new AdminSalesViewModel
            {
                SearchId = searchId,
                SearchDate = searchDate,
                SearchPrice = searchPrice,
                SearchUser = searchUser,
                SearchBoxOffice = searchBoxOffice
            };

            //Aunamos todos los componentes necesarios para la busqueda en una sola consulta con include(cargando todos los datos)
            //y la hacemos iqueryable para no meterla en memoria aún y poder seguir modificandola.
            var query = _db.Tickets
                .Include(c => c.Session).ThenInclude(s => s!.Movie)
                .Include(c => c.Session).ThenInclude(s => s!.Room)
                .Include(c => c.Customer)
                .Include(c => c.TicketSeats)
                .Include(c => c.TicketAddOns).ThenInclude(z => z.AddOn)
                .AsQueryable();

            // Si alguno de los filtros ha sido rellenado por el Admin...
            if (searchId.HasValue || searchDate.HasValue || searchPrice.HasValue || !string.IsNullOrEmpty(searchUser) || searchBoxOffice.HasValue)
            {
                //Marcamos la propiedad de busqueda a true para que active la busqueda en la vista (aparezcan componentes)
                viewModel.SearchPerformed = true;
                //Aplicamos los filtros en la consulta para cada caso
                if (searchId.HasValue) query = query.Where(t => t.TicketId == searchId);
                if (searchDate.HasValue) query = query.Where(t => t.PurchasedAt.HasValue
                    && t.PurchasedAt.Value.Date == searchDate.Value.Date);
                if (searchPrice.HasValue) query = query.Where(t => t.TotalPrice == searchPrice);
                if (!string.IsNullOrEmpty(searchUser)) query = query.Where(t => t.Customer!.FirstName!.Contains(searchUser) || t.EmailToSend!.Contains(searchUser));
                if (searchBoxOffice.HasValue) query = query.Where(t => t.SoldAtBoxOffice == searchBoxOffice.Value);
            }

            //Aqui ya SI vamos a la base de datos para aplicar la consulta con los filtros, ordenandola por dia 
            var resultsList = await query.OrderByDescending(v => v.PurchasedAt).ToListAsync();

            //Metemos la lista de tickets paginada en la propiedad FoundTicket del VM (La propiedad es una lista paginada)
            viewModel.FoundTickets = resultsList.ToPagedList(pageNumber, pageSize);

            // Consulta para traer la recaudacion por película (precio de asientos vendidos).
            var movieTickets = await _db.Tickets
                .Where(b => b.Session != null && b.Session.Movie != null)
                .Select(c => new {
                    Title = c.Session!.Movie!.Title,
                    SeatsCount = c.TicketSeats.Count,
                    SeatPrice = c.Session.Price
                }).ToListAsync();

            // Con la consulta de antes, simplemente agrupamos por titulo y en un VM de MovieSales metemos
            //El titulo, El conteo de ventas y el total de ventas de asientos (precios)
            viewModel.MovieSales = movieTickets
                .GroupBy(x => x.Title)
                .Select(g => new MovieSales
                {
                    MovieTitle = g.Key!,
                    TicketsCount = g.Count(), // Número de operaciones/ventas
                    TotalSeats = g.Sum(x => x.SeatsCount * x.SeatPrice) // Solo cogemos los asientos
                })
                .OrderByDescending(x => x.TotalSeats)
                .ToList();

            // Consulta para recoger las ventas de los complementos (Nombre y precio)
            var addonsData = await _db.TicketAddOns
                .Include(r => r.AddOn)
                .Where(n => n.AddOn != null)
                .Select(c => new 
                { 
                    Name = c.AddOn!.AddOnName,
                    Price = c.AddOn.Price
                }).ToListAsync();

            //Con la consulta de antes, los ordenamos por nombre y seleccionamos en un VM de AddOnSales
            //Nombre, conteo total de unidades y el precio total de estas
            viewModel.AddOnSales = addonsData.GroupBy(x => x.Name)
                .Select(g => new AddOnSales
                {
                    AddOnName = g.Key!,
                    SoldUnits = g.Count(),
                    TotalRevenue = g.Sum(x => x.Price)
                }).OrderByDescending(x => x.SoldUnits).ToList();

            return View(viewModel);
        }
    }
}
