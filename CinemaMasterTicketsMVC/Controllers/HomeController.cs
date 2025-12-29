using CinemaMasterTicketsMVC.Data;
using CinemaMasterTicketsMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CinemaMasterTicketsMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        //En el index de Home, directamente cargamos el contenido de las peliculas que ya tengan sesiones activas.
        //Solo 10 ya que solo tendremos 10 salas
        public IActionResult Index()
        {
            //Primero limpiamos todo el contenido de la Session para evitar residuos por si el usuario decide
            //volver a la pagina de inicio desde algun punto de la compra.
            HttpContext.Session.Clear();
            var model = _db.Movies
        .Include(m => m.Sessions) // Cargamos las sesiones
        .Where(m =>
            m.BackdropUrl != null &&
            m.Sessions.Any(s =>
                s.Status == "Active" &&
                s.StartTime > DateTime.Now) //Aqui filtramos para que solo aparezcan las sesiones posteriores a la fecha actual
        )
        .OrderByDescending(m => m.AddedAt)
        .Take(10)
        .ToList();

            return View(model);
        }
        //Aviso de privacidad
        public IActionResult Privacy()
        {
            return View();
        }
        //Terminos legales
        public IActionResult Legal()
        {
            return View();
        }

    }
}
