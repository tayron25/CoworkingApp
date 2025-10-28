using CoworkingApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoworkingApp.Data
{
    // Cambia IdentityDbContext por IdentityDbContext<ApplicationUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Agrega un DbSet para cada una de tus nuevas tablas
        public DbSet<TipoEspacio> TiposEspacio { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<CreditPackage> CreditPackages { get; set; }
    }
}