namespace CoworkingApp.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal CreditosDisponibles { get; set; }
        public List<Reserva> ProximasReservas { get; set; } // <-- Nuevo
        public List<Reserva> ReservasPasadas { get; set; }  // <-- Nuevo

        public DashboardViewModel()
        {
            ProximasReservas = new List<Reserva>();
            ReservasPasadas = new List<Reserva>();
        }
    }
}