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
        public async Task<IActionResult> Index(int? id)
        {
            Employee? employee;

            if (id.HasValue)
            {
                // El Admin está consultando un perfil específico
                employee = await _db.Employees
                    .Include(e => e.BoxOffice)
                    .Include(e => e.Tickets)
                    .FirstOrDefaultAsync(e => e.EmployeeId == id.Value);
            }
            else
            {
                // El empleado logueado entra a su propio panel
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> UploadPhoto(IFormFile ProfileImageFile)
        {
            var user = await _userManager.GetUserAsync(User);
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);

            if (employee != null && ProfileImageFile != null && ProfileImageFile.Length > 0)
            {
                // 1. Definir carpeta y asegurar que existe
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/employees");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // 2. Generar nombre único
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImageFile.FileName);
                string fullPath = Path.Combine(folderPath, fileName);

                // 3. Guardar el archivo
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await ProfileImageFile.CopyToAsync(stream);
                }

                // 4. Borrar foto anterior si no es la de por defecto
                if (!string.IsNullOrEmpty(employee.ProfileImage))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", employee.ProfileImage);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // 5. Actualizar la ruta en la base de datos
                employee.ProfileImage = "img/employees/" + fileName;
                await _db.SaveChangesAsync();

                TempData["Message"] = "¡Foto de perfil actualizada con éxito!";
            }

            return RedirectToAction(nameof(Index));
        }
        
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            Employee? employee = null;

            // 1. DETERMINAR QUÉ EMPLEADO BUSCAR
            if (id.HasValue && User.IsInRole("Admin"))
            {
                // Si hay ID y soy Admin, busco al empleado por ese ID
                employee = await _db.Employees.FindAsync(id.Value);
            }
            else
            {
                // Si no hay ID (o soy empleado), busco MI PROPIO perfil por mi Email
                var user = await _userManager.GetUserAsync(User);
                employee = await _db.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);
            }

            if (employee == null) return NotFound();

            //Prepara el view model para el BoxOffice
            var boxOficceDisplay = _db.BoxOffices.Select(b => new
            {
                id = b.BoxOfficeId,
                value = b.BoxOfficeName
            });
            var model = new CreateEmployeeBoxOficceViewModel
            {
                Employee= employee,
                BoxOffices = new SelectList(boxOficceDisplay, "id", "value")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateEmployeeBoxOficceViewModel model)
        {
            // Buscamos la entidad real en la DB para no perder datos que no están en el form (como la imagen)
            var employeeInDb = await _db.Employees.FindAsync(model.Employee.EmployeeId);
            if (employeeInDb == null) return NotFound();

            // Actualizamos solo los campos permitidos (Pattern de Customer)
            employeeInDb.Firstname = model.Employee.Firstname;
            employeeInDb.Lastname = model.Employee.Lastname;
            employeeInDb.DNI = model.Employee.DNI;

            // Solo el Admin puede cambiar la taquilla
            if (User.IsInRole("Admin"))
            {
                employeeInDb.BoxOfficeId = model.Employee.BoxOfficeId;
            }

            // Como solo actualizamos campos específicos de la entidad trackeada, 
            // no necesitamos validar ProfileImageFile ni campos que no enviamos.
            if (ModelState.IsValid)
            {
                _db.Update(employeeInDb);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Datos actualizados correctamente.";
                if (User.IsInRole("Admin"))
                {
                    // Si eres Admin, vuelve a la lista general de empleados 
                    // (Asegúrate de que esta acción existe, ej: List o AdminIndex)
                    return RedirectToAction("Index", "Admin");
                    // O si tu lista está en el mismo controller pero en otra acción:
                    // return RedirectToAction(nameof(List));
                }

                return RedirectToAction(nameof(Index));
            }

            // Si hay error de validación, recargar lista
            var boxOficceDisplay = _db.BoxOffices.Select(b => new { id = b.BoxOfficeId, value = b.BoxOfficeName });
            model.BoxOffices = new SelectList(boxOficceDisplay, "id", "value", model.Employee.BoxOfficeId);

            return View(model);
        }
    }
}
