using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaMasterTicketsMVC.Controllers
{
    [Authorize(Roles = "Customer,Admin")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        

        public CustomerController(ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(int? id)
        {
            Customer? customer;

            if (id.HasValue)
            {
                // El Admin está consultando la ficha de un cliente
                customer = await _db.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == id.Value);
            }
            else
            {
                // El cliente está viendo su propio perfil
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                customer = await _db.Customers
                    .FirstOrDefaultAsync(c => c.Email == user.Email);
            }

            if (customer == null) return NotFound();

            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(Customer model)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _db.Customers.FirstAsync(c => c.Email == user!.Email);

            // Verificamos que el archivo exista y no esté vacío
            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                // 1. Lógica para borrar la foto anterior si existe
                if (!string.IsNullOrEmpty(customer.ProfileImage))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", customer.ProfileImage);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // 2. Lógica para guardar la nueva foto
                var fileName = $"{customer.CustomerId}_{Guid.NewGuid()}_{Path.GetFileName(model.ProfileImageFile.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/profiles", fileName);

                // Asegurar que la carpeta existe
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory!);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImageFile.CopyToAsync(stream);
                }

                // 3. Actualizar ruta en BD
                customer.ProfileImage = $"img/profiles/{fileName}";
                await _db.SaveChangesAsync();

                TempData["Message"] = "Foto de perfil actualizada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        

        // GET: Customer/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == user!.Email);

            if (customer == null) return NotFound();

            return View(customer);
        }

        // POST: Customer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer model)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == user!.Email);

            if (customer == null) return NotFound();

            // Solo editamos los campos permitidos (no editamos el Email ni el ID por seguridad)
            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Address = model.Address;
            customer.City = model.City;

            if (ModelState.IsValid)
            {
                _db.Update(customer);
                await _db.SaveChangesAsync();
                TempData["Message"] = "Perfil actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        // darse de baja-> eliminar su registro de bd y de identity
        [Authorize]
        public async Task<IActionResult> Unsubscribe()
        {
            // Obtenemos el email o ID del usuario actual mediante Claims
            var userEmail = User.Identity!.Name;
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return NotFound();

            return View(customer);
            
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UnsubscribeConfirmed()
        {
            var userEmail = User!.Identity!.Name;
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == userEmail);
            var user = await _userManager.FindByEmailAsync(userEmail!);

            if (customer == null || user == null) return NotFound();

            
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                
                customer.isActive = false;

                _db.Customers.Update(customer);
                await _db.SaveChangesAsync();

                
                await _signInManager.SignOutAsync();

                
                return RedirectToAction("Index", "Home");
            }

            return View("Error");
        }
    }

}
