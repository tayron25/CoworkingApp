using CoworkingApp.Data;
using CoworkingApp.Models;
using Microsoft.AspNetCore.Authorization; // Importante para la seguridad
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // Para obtener el ID de usuario

namespace CoworkingApp.Controllers
{
    [Authorize] // Solo usuarios logueados pueden acceder a este controlador
    public class ReservaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reserva/Crear/5
        public async Task<IActionResult> Crear(int tipoEspacioId)
        {
            var tipoEspacio = await _context.TiposEspacio.FindAsync(tipoEspacioId);
            if (tipoEspacio == null)
            {
                return NotFound();
            }

            // --- INICIO DE LA SOLUCIÓN ---
            // Creamos una reserva con valores por defecto para las fechas.
            var reserva = new Reserva
            {
                TipoEspacioId = tipoEspacioId,
                FechaInicio = DateTime.Now, // Fecha y hora actual como inicio
                FechaFin = DateTime.Now.AddHours(1) // Una hora después como fin
            };
            // --- FIN DE LA SOLUCIÓN ---

            ViewBag.TipoEspacioNombre = tipoEspacio.Nombre;
            ViewBag.HorariosOcupados = await _context.Reservas
                .Where(r => r.TipoEspacioId == tipoEspacioId && r.FechaFin > DateTime.Now)
                .OrderBy(r => r.FechaInicio)
                .ToListAsync();

            return View(reserva); // Pasamos el objeto con las fechas ya establecidas
        }

        // POST: Reserva/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Reserva reserva)
        {
            var userId = _userManager.GetUserId(User);
            var usuarioActual = await _userManager.FindByIdAsync(userId);
            var tipoEspacio = await _context.TiposEspacio.FindAsync(reserva.TipoEspacioId);

            if (tipoEspacio == null) return NotFound();

            reserva.UsuarioId = userId;

            // --- INICIO DE NUEVAS VALIDACIONES ---

            // 1. Validar que la reserva empiece en una hora exacta (minutos y segundos son 0)
            if (reserva.FechaInicio.Minute != 0 || reserva.FechaInicio.Second != 0 || reserva.FechaFin.Minute != 0 || reserva.FechaFin.Second != 0)
            {
                ModelState.AddModelError(string.Empty, "Las reservas solo pueden empezar y terminar en horas exactas (ej. 10:00, 14:00).");
            }

            // 2. Validar que la duración sea de al menos una hora
            if ((reserva.FechaFin - reserva.FechaInicio).TotalHours < 1)
            {
                ModelState.AddModelError("FechaFin", "La duración mínima de la reserva es de una hora.");
            }

            // 3. Validar que la fecha de fin sea posterior a la de inicio (esto ya lo teníamos)
            if (reserva.FechaFin <= reserva.FechaInicio)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            // 4. VALIDACIÓN DE DISPONIBILIDAD (la más importante)
            // Buscamos si ya existe alguna reserva para este tipo de espacio que se cruce con el horario solicitado.
            var isOccupied = await _context.Reservas
                .AnyAsync(r => r.TipoEspacioId == reserva.TipoEspacioId &&
                               r.FechaInicio < reserva.FechaFin &&
                               r.FechaFin > reserva.FechaInicio);

            if (isOccupied)
            {
                ModelState.AddModelError(string.Empty, "El horario seleccionado ya no está disponible. Por favor, elige otro.");
            }

            // --- FIN DE NUEVAS VALIDACIONES ---

            if (ModelState.IsValid)
            {
                double totalHoras = (reserva.FechaFin - reserva.FechaInicio).TotalHours;
                decimal costoTotal = (decimal)totalHoras * tipoEspacio.CostoCreditosHora;

                if (usuarioActual.CreditosDisponibles >= costoTotal)
                {
                    usuarioActual.CreditosDisponibles -= costoTotal;
                    _context.Add(reserva);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "¡Tu reserva ha sido confirmada con éxito! ✅";
                    return RedirectToAction("Index", "Dashboard"); // Redirigimos al Dashboard
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Créditos insuficientes. Necesitas {costoTotal:F2} y solo tienes {usuarioActual.CreditosDisponibles:F2}.");
                }
            }

            // Si llegamos aquí, algo falló. Volvemos a mostrar el formulario.
            ViewBag.TipoEspacioNombre = tipoEspacio.Nombre;
            // Pasamos la lista de horarios ocupados a la vista para que el usuario pueda verlos
            ViewBag.HorariosOcupados = await _context.Reservas
                .Where(r => r.TipoEspacioId == reserva.TipoEspacioId && r.FechaFin > DateTime.Now)
                .OrderBy(r => r.FechaInicio)
                .ToListAsync();

            return View(reserva);
        }

        [HttpGet]
        public async Task<IActionResult> GetOccupiedHours(int tipoEspacioId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            // Se obtienen las reservas que se cruzan con el día seleccionado
            var reservationsThisDay = await _context.Reservas
                .Where(r => r.TipoEspacioId == tipoEspacioId &&
                           r.FechaInicio < endOfDay &&
                           r.FechaFin > startOfDay)
                .ToListAsync();

            var occupiedHours = new List<int>();

            // --- INICIO DE LA SOLUCIÓN ---
            // Se itera por cada hora del día (0-23)
            for (int hour = 0; hour < 24; hour++)
            {
                // Se define el timespan para la hora actual (ej. de 14:00 a 15:00)
                var slotStart = startOfDay.AddHours(hour);
                var slotEnd = slotStart.AddHours(1);

                // Se comprueba si alguna de las reservas del día se solapa con este slot de una hora.
                // Esta es la forma estándar y más segura de comprobar colisiones.
                bool isSlotOccupied = reservationsThisDay.Any(r => r.FechaInicio < slotEnd && r.FechaFin > slotStart);

                if (isSlotOccupied)
                {
                    occupiedHours.Add(hour);
                }
            }
            // --- FIN DE LA SOLUCIÓN ---

            return Json(occupiedHours);
        }


    }
}