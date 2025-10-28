using CoworkingApp.Data;
using CoworkingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Controllers
{
    [Authorize] // Todos los usuarios logueados pueden ver la tienda
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoreController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Muestra todos los paquetes activos
        public async Task<IActionResult> Index()
        {
            var packages = await _context.CreditPackages.Where(p => p.IsActive).ToListAsync();
            return View(packages);
        }

        // Procesa la "compra"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int packageId)
        {
            var package = await _context.CreditPackages.FindAsync(packageId);
            var user = await _userManager.GetUserAsync(User);

            if (package != null && user != null && package.IsActive)
            {
                // Simplemente añade los créditos del paquete al usuario
                user.CreditosDisponibles += package.Credits;
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = $"¡Se han añadido {package.Credits} créditos a tu cuenta! 💰";
            }

            // Redirige al dashboard para que vea su nuevo saldo
            return RedirectToAction("Index", "Dashboard");
        }
    }
}