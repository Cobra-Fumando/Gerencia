using Microsoft.EntityFrameworkCore;
using Serviços.Tabelas;

namespace Serviços.Connection
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuarios> Usuarios { get; set; }
    }
}
