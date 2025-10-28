using CoworkingApp.Data; // Añadir esto
using CoworkingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Añadir esto
using System.Diagnostics;

namespace CoworkingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context; // Añadir esto

        // Modificar constructor
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Modificar la acción Index
        public async Task<IActionResult> Index()
        {
            var listaEspacios = await _context.TiposEspacio.ToListAsync();
            return View(listaEspacios);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}