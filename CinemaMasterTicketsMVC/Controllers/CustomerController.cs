using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaMasterTicketsMVC.Controllers
{
    // Solo permitimos la entrada a usuarios con el rol de Cliente o Admin
    [Authorize(Roles = "Customer,Admin")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        // Constructor para inyectar la base de datos y los gestores de Identity (usuarios y sesiones)
        public CustomerController(ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        //El metodo index mostrara el perfil. Si viene con un ID, es el Admin viendo un perfil.
        // Si no trae ID, es el propio cliente viendo sus datos.
        public async Task<IActionResult> Index(int? id)
        {
            Customer? customer;

            if (id.HasValue)
            {
                // Si hay ID, buscamos al cliente directamente por su clave primaria
                customer = await _db.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == id.Value);
            }
            else
            {
                // Si no hay ID, buscamos quien es el usuario que tiene la sesión abierta ahora mismo
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                //buscamos en nuestra tabla de Clientes el que coincida con el email del usuario logueado
                customer = await _db.Customers
                    .FirstOrDefaultAsync(c => c.Email == user.Email);
            }

            if (customer == null) return NotFound();

            return View(customer);
        }

        //Metodo que controla la subida de la foto de perfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(Customer model)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _db.Customers.FirstAsync(c => c.Email == user!.Email);

            // Verificamos que el archivo exista y no este vacio
            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                // si ya existia o tenia una foto antes la borramos par no acumular datos innecesarios 
                if (!string.IsNullOrEmpty(customer.ProfileImage))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", customer.ProfileImage);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Para guardar la foto-> creamos un nombre unico para la imagen usando un GUID para evitar
                // que dos fotos se llamen igual
                var fileName = $"{customer.CustomerId}_{Guid.NewGuid()}_{Path.GetFileName(model.ProfileImageFile.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/profiles", fileName);

                // Asegurar que la carpeta existe
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory!);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImageFile.CopyToAsync(stream);
                }

                // En la base de datos guardamos solo la ruta relativa para poder mostrarla luego en la etiqueta <img>
                customer.ProfileImage = $"img/profiles/{fileName}";
                await _db.SaveChangesAsync();

                TempData["Message"] = "Foto de perfil actualizada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        

        //Vista para editar los datos del perfil
        public async Task<IActionResult> Edit()
        {
            //buscamos el usuario actual
            var user = await _userManager.GetUserAsync(User);
            //buscamos el cliente buscando coincidencia de email en la bd y en la bd de identity
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == user!.Email);
            //validacion si no se encuentra el cliente
            if (customer == null) return NotFound();
            //se envia el objeto cliente a la vista
            return View(customer);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer model)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == user!.Email);

            if (customer == null) return NotFound();

            // Guardamos los datos del formulario en el objeto aqui el Email y el ID
            // no se tocan aquí por seguridad.
            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Address = model.Address;
            customer.City = model.City;

            //si las validaciones del modelo son correctas se procede a guardar los cambios 
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
        //vista de confirmacion de la baja para que el usuario piense si quiere irse
        [Authorize]
        public async Task<IActionResult> Unsubscribe()
        {
            // Obtenemos el email o ID del usuario actual
            var userEmail = User.Identity!.Name;
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return NotFound();

            return View(customer);
            
        }

        //Borrado definitivo-> borra al usuario del sistema tanto de identity y de la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UnsubscribeConfirmed()
        {
            var userEmail = User!.Identity!.Name;
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Email == userEmail);
            var user = await _userManager.FindByEmailAsync(userEmail!);

            if (customer == null || user == null) return NotFound();

            // primero borramos de identity-> que es el que gestiona el login
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                // Si se borro bien de identity lo borramos de nuestra bd Customers
                _db.Customers.Remove(customer);
                await _db.SaveChangesAsync();

                // Al borrar su cuenta su cookie ya no vale, cerramos su sesion manualmente 
                //para que el navegador sepa que ya no esta dentro
                await _signInManager.SignOutAsync();

                //Redirigimos a home 
                return RedirectToAction("Index", "Home");
            }

            return View("Error"); //Si falla el darse de baja
        }
    }

}
