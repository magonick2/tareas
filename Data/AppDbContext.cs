using Microsoft.EntityFrameworkCore;
using TareasApi.Models;

namespace TareasApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Tarea> Tareas { get; set; }
    }
}