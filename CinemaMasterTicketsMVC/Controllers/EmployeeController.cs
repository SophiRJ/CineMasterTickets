                                                                                                                                      using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
using CinemaMasterTicketsMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CinemaMasterTicketsMVC.Controllers
{
    //Solo permitimos el acceso a usuarios que sean Empleados o Administradores
    [Authorize(Roles = "Employee,Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public EmployeeController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // Este metodo muestra el perfil del empleado.
        // Si recibe un id es el Admin consultando a un empleado específico.
        //Si no recibe nada el empleado está viendo su propio perfil
        public async Task<IActionResult> Index(int? id)
        {
            Employee? employee;

            if (id.HasValue)
            {
                // El Admin esta consultando un perfil
                //se busca al empleado por id y se incluye tickets y taquilla
                employee = await _db.Employees
                    .Include(e => e.BoxOffice)
                    .Include(e => e.Tickets)
                    .FirstOrDefaultAsync(e => e.EmployeeId == id.Value);
            }
            else
            {
                // El empleado logueado entra a su propio perfil
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                employee = await _db.Employees
                    .Include(e => e.BoxOffice)
                    .Include(e => e.Tickets)
                    .FirstOrDefaultAsync(e => e.Email == user.Email);
            }

            if (employee == null) return NotFound();

            return View(employee);
        }

        //Metodo para procesar la subida de la foto de perfil
        //Solo el empleado puede ejecutar esta accion 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> UploadPhoto(IFormFile ProfileImageFile)
        {
            var user = await _userManager.GetUserAsync(User);
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);

            if (employee != null && ProfileImageFile != null && ProfileImageFile.Length > 0)
            {
                // Definimos carpeta y nos aseguramos que existe que existe, si no existe se creara.
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/employees");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                //Generamos nombre unico para que no haya conflictos de duplicados
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImageFile.FileName);
                string fullPath = Path.Combine(folderPath, fileName);

                // Guardamos el archivo
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await ProfileImageFile.CopyToAsync(stream);
                }

                //Borramos la foto anterior
                if (!string.IsNullOrEmpty(employee.ProfileImage))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", employee.ProfileImage);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // Actualizamos la ruta relativa en la base de datos para mostrarla luego en la vista
                employee.ProfileImage = "img/employees/" + fileName;
                await _db.SaveChangesAsync();

                TempData["Message"] = "¡Foto de perfil actualizada con éxito!";
            }

            return RedirectToAction(nameof(Index));
        }

        //Este metodo devuelve la vista para editar los datos del empleado
        
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            Employee? employee = null;

            // Validamos que que perfil cargar segun quien este navegando o Admin o empleado
            if (id.HasValue && User.IsInRole("Admin"))
            {
                // Si hay id y soy Admin, se busca al empleado por el id
                employee = await _db.Employees.FindAsync(id.Value);
            }
            else
            {
                // Si es empleado se busca su perfil mediante su email
                var user = await _userManager.GetUserAsync(User);
                employee = await _db.Employees.FirstOrDefaultAsync(e => e.Email == user!.Email);
            }

            if (employee == null) return NotFound();

            //Prepara la lista de taquillas disponibles para el dropdown
            var boxOficceDisplay = _db.BoxOffices.Select(b => new
            {
                id = b.BoxOfficeId,
                value = b.BoxOfficeName
            });
            //Creamos el viewmodel que contiene al empleado y la lista de taquillas
            var model = new CreateEmployeeBoxOficceViewModel
            {
                Employee= employee,
                BoxOffices = new SelectList(boxOficceDisplay, "id", "value")
            };

            return View(model);
        }

        //Este metodo guarda los cambios que devuelve la vista con el formulario de edicion 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateEmployeeBoxOficceViewModel model)
        {
            // Buscamos al empleado en la base de datos 
            var employeeInDb = await _db.Employees.FindAsync(model.Employee!.EmployeeId);
            if (employeeInDb == null) return NotFound();

            //Actualizamos los campos permitidos
            employeeInDb.Firstname = model.Employee.Firstname;
            employeeInDb.Lastname = model.Employee.Lastname;
            employeeInDb.DNI = model.Employee.DNI;

            // Este campo que es la taquilla del empleado solo puede ser cambiada por el administrador
            if (User.IsInRole("Admin"))
            {
                employeeInDb.BoxOfficeId = model.Employee.BoxOfficeId;
            }

            // Si el modelo es valido
            if (ModelState.IsValid)
            {
                _db.Update(employeeInDb);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Datos actualizados correctamente.";
                if (User.IsInRole("Admin"))
                {
                    // Si le enviamos a la lista de empleados y clientes
                    return RedirectToAction("Index", "Admin");
                }
                //Si el empleado se edito a si mismo vulve a su perfil
                return RedirectToAction(nameof(Index));
            }

            // Si hay error de validacion recargamos la lista de las taquillas 
            var boxOficceDisplay = _db.BoxOffices.Select(b => new { id = b.BoxOfficeId, value = b.BoxOfficeName });
            model.BoxOffices = new SelectList(boxOficceDisplay, "id", "value", model.Employee.BoxOfficeId);

            return View(model);
        }
    }
}
