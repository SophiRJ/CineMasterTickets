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
        public IActionResult Home()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminUsersPanelViewModel
            {
                // Traemos empleados con su taquilla incluida
                Employees = await _db.Employees
            .Include(e => e.BoxOffice)
            .OrderBy(e => e.Lastname)
            .ToListAsync(),

                // Traemos clientes
                Customers = await _db.Customers
            .OrderBy(c => c.LastName)
            .ToListAsync()
            };

            return View(model);
        }


        public IActionResult Roles()
        {
            //todos los roles que haya en el roleManager
            var roles = _roleManager!.Roles;
            return View(roles);
        }


        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string userEmail, string roleName)
        {
            var user = await _userManager!.FindByEmailAsync(userEmail);

            if (user != null && await _roleManager!.RoleExistsAsync(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateEmployee()
        {
            var boxOficceDisplay = _db.BoxOffices.Select(b => new
            {
                id = b.BoxOfficeId,
                value = b.BoxOfficeName
            });
            var model = new CreateEmployeeBoxOficceViewModel
            {
                BoxOffices = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(boxOficceDisplay, "id", "value")
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeBoxOficceViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var boxOficceDisplay = _db.BoxOffices.Select(b => new
                {
                    id = b.BoxOfficeId,
                    value = b.BoxOfficeName
                });
                var model = new CreateEmployeeBoxOficceViewModel
                {
                    BoxOffices = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(boxOficceDisplay, "id", "value")
                };
                return View(model);
            }

            var pass = vm.Employee!.Lastname + "123!";
            // Crear Identity User
            var user = new IdentityUser
            {
                UserName = vm.Employee.Email,
                Email = vm.Employee.Email,
                EmailConfirmed = true
            };

            var result = await _userManager!.CreateAsync(user, pass);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(vm);
            }

            //Asignar rol Employee
            await _userManager.AddToRoleAsync(user, "Employee");

            _db.Employees.Add(vm.Employee);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployeeAjax(int id)
        {
            // 1. Buscar al empleado
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
                return Json(new { success = false, message = "Empleado no encontrado." });

            try
            {
                // 2. Borrar el usuario de Identity (basado en el Email)
                var user = await _userManager!.FindByEmailAsync(employee.Email!);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        return Json(new { success = false, message = "Error al eliminar el usuario de acceso." });
                    }
                }

                // 3. Borrar de la base de datos de empleados
                _db.Employees.Remove(employee);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Empleado eliminado correctamente." });
            }
            catch (Exception ex)
            {
                // Loguear el error si es necesario
                return Json(new { success = false, message = "Error interno: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomerAjax(int id)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
                return Json(new { success = false, message = "Cliente no encontrado" });

            try
            {
                // Borrar de identity
                var user = await _userManager!.FindByEmailAsync(customer.Email!);
                if (user != null)
                    await _userManager.DeleteAsync(user);

                // Borrar de base de datos local
                _db.Customers.Remove(customer);
                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Cliente eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error en el servidor: " + ex.Message });
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

                    //El movimiento siguiente es importante e interesante:
                    //Por cada pelicula se van pidiendo los detalles necesarios que no encontramos en la llamada
                    //de la URL normal (hay que hacer una segunda llamada para acceder a datos como el director o la 
                    //duracion de la pelicula).
                    foreach (var movie in movies)
                    {
                        try
                        {
                            //Llamada a la url para los detalles de la pelicula con GetAsync
                            string detailUrl = $"https://api.themoviedb.org/3/movie/{movie.id}?api_key={apiKey}&language=es-ES&append_to_response=credits";
                            var detailResponse = await httpClient.GetAsync(detailUrl);

                            //Si la llamada tieene exito...
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

        public async Task<IActionResult> GetAddons()
        {
            var addons = await _db.AddOns.ToListAsync();

            return View(addons);
        }

        public IActionResult AddAddon()
        {
            // Preparar dropdown de tipos
            ViewBag.AddOnTypes = Enum.GetValues(typeof(AddOnType))
                                     .Cast<AddOnType>()
                                     .Select(a => new SelectListItem
                                     {
                                         Value = a.ToString(),
                                         Text = a.ToString()
                                     }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddon(AddOn addOn)
        {
            // Preparar dropdown nuevamente en caso de error
            ViewBag.AddOnTypes = Enum.GetValues(typeof(AddOnType))
                                     .Cast<AddOnType>()
                                     .Select(a => new SelectListItem
                                     {
                                         Value = a.ToString(),
                                         Text = a.ToString()
                                     }).ToList();

            if (ModelState.IsValid)
            {
                if (addOn.AddOnImageFile != null && addOn.AddOnImageFile.Length > 0)
                {
                    // Crear ruta de la imagen
                    var fileName = Path.GetFileName(addOn.AddOnImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/addons", fileName);

                    // Guardar archivo en wwwroot/img/addons
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await addOn.AddOnImageFile.CopyToAsync(stream);
                    }

                    // Guardar la ruta relativa en la base de datos
                    addOn.AddOnImage = $"img/addons/{fileName}";
                }

                // Guardar en la base de datos
                _db.AddOns.Add(addOn);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(GetAddons));

            }

            return View(addOn);
        }

        
        public async Task<IActionResult> EditAddOn(int id)
        {
            var addOn = await _db.AddOns.FindAsync(id);
            if (addOn == null)
                return NotFound();

            ViewBag.AddOnTypes = Enum.GetValues(typeof(AddOnType))
                .Cast<AddOnType>()
                .Select(a => new SelectListItem
                {
                    Value = a.ToString(),
                    Text = a.ToString()
                }).ToList();

            return View(addOn);
        }

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

            if (!ModelState.IsValid)
                return View(addOn);

            var addOnDb = await _db.AddOns.FindAsync(id);
            if (addOnDb == null)
                return NotFound();

            // Actualizar campos
            addOnDb.AddOnName = addOn.AddOnName;
            addOnDb.Price = addOn.Price;
            addOnDb.Type = addOn.Type;

            // Si se sube nueva imagen
            if (addOn.AddOnImageFile != null && addOn.AddOnImageFile.Length > 0)
            {

                //Borrar imagen antigua de la carpeta
                if (!string.IsNullOrEmpty(addOnDb.AddOnImage))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", addOnDb.AddOnImage);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }


                //Guardar nueva imagen
                var fileName = Path.GetFileName(addOn.AddOnImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/addons", fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await addOn.AddOnImageFile.CopyToAsync(stream);

                addOnDb.AddOnImage = $"img/addons/{fileName}";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(GetAddons));
        }
        public async Task<IActionResult> DeleteAddOn(int id)
        {
            var addOn = await _db.AddOns.FindAsync(id);
            if (addOn == null)
                return NotFound();

            return View(addOn);
        }
        [HttpPost, ActionName("DeleteAddOn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var addOn = await _db.AddOns.FindAsync(id);
            if (addOn == null)
                return NotFound();

            // (Opcional) borrar imagen física
            if (!string.IsNullOrEmpty(addOn.AddOnImage))
            {
                var imagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    addOn.AddOnImage
                );

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _db.AddOns.Remove(addOn);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(GetAddons));
        }


    }
}
