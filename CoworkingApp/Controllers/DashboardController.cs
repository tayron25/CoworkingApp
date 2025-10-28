using CoworkingApp.Data;
using CoworkingApp.Models;
using CoworkingApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Controllers
{
    [Authorize] // Solo usuarios logueados pueden acceder
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var todasLasReservas = await _context.Reservas
                                             .Include(r => r.TipoEspacio)
                                             .Where(r => r.UsuarioId == currentUser.Id)
                                             .OrderByDescending(r => r.FechaInicio)
                                             .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                CreditosDisponibles = currentUser.CreditosDisponibles,
                // Separa las reservas usando LINQ
                ProximasReservas = todasLasReservas.Where(r => r.FechaFin > DateTime.Now).ToList(),
                ReservasPasadas = todasLasReservas.Where(r => r.FechaFin <= DateTime.Now).ToList()
            };

            return View(viewModel);
        }
        // POST: Elimina una reserva individual del usuario logueado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReservation(int reservationId)
        {
            var userId = _userManager.GetUserId(User);
            var reservation = await _context.Reservas.FindAsync(reservationId);

            // Doble comprobación: la reserva existe y pertenece al usuario actual
            if (reservation != null && reservation.UsuarioId == userId)
            {
                _context.Reservas.Remove(reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}