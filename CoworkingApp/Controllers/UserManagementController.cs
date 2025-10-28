using CoworkingApp.Data;
using CoworkingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context; // <-- Añade esta línea

        // Modifica el constructor para que acepte el DbContext
        public UserManagementController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context; // <-- Añade esta línea
        }

        // Muestra la lista de todos los usuarios
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // Acción para promover a un usuario a Admin
        [HttpPost]
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Se asegura de que no esté ya en el rol antes de añadirlo
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
            return RedirectToAction("Index");
        }

        // Acción para quitarle el rol de Admin a un usuario
        [HttpPost]
        public async Task<IActionResult> DemoteFromAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // No te puedes quitar el rol de admin a ti mismo
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser.Id != user.Id)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserReservations(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var userReservations = _context.Reservas.Where(r => r.UsuarioId == userId);

            if (userReservations.Any())
            {
                _context.Reservas.RemoveRange(userReservations);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        // GET: Muestra todas las reservas de un usuario específico
        public async Task<IActionResult> ViewReservations(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userReservations = await _context.Reservas
                .Where(r => r.UsuarioId == userId)
                .Include(r => r.TipoEspacio) // Para mostrar el nombre del espacio
                .OrderByDescending(r => r.FechaInicio)
                .ToListAsync();

            ViewBag.UserEmail = user.Email; // Para mostrar en el título de la página
            ViewBag.UserId = user.Id; // Para la redirección
            return View(userReservations);
        }

        // POST: Elimina una reserva individual
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReservation(int reservationId, string userId)
        {
            var reservation = await _context.Reservas.FindAsync(reservationId);
            if (reservation != null)
            {
                _context.Reservas.Remove(reservation);
                await _context.SaveChangesAsync();
            }

            // Redirige de vuelta a la lista de reservas del mismo usuario
            return RedirectToAction("ViewReservations", new { userId = userId });
        }
    }
}