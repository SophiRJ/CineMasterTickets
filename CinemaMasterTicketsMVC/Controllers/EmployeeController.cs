using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaMasterTicketsMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public EmployeeController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var employee = await _db.Employees
                .Include(e => e.BoxOffice)
                .Include(e => e.Tickets) // IMPORTANTE: Para que VentasHoy funcione
                .FirstOrDefaultAsync(e => e.Email == user.Email);

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
        // GET: Employee/Edit
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Email == user.Email);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Employee/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(Employee model)
        {
            // Buscamos el empleado original para asegurar que no tocamos campos sensibles
            var employeeInDb = await _db.Employees.FindAsync(model.EmployeeId);

            if (employeeInDb == null) return NotFound();

            // Quitamos la validación de la imagen ya que no se envía en este formulario
            ModelState.Remove("ProfileImageFile");

            if (ModelState.IsValid)
            {
                // Actualizamos solo los datos permitidos
                employeeInDb.Firstname = model.Firstname;
                employeeInDb.Lastname = model.Lastname;
                employeeInDb.DNI = model.DNI;

                _db.Update(employeeInDb);
                await _db.SaveChangesAsync();

                TempData["Message"] = "Datos actualizados correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
