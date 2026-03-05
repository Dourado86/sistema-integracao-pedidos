using Microsoft.EntityFrameworkCore;
using IntegracaoPedidos.Core.Models;

namespace IntegracaoPedidos.Infrastructure.Data

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pedido> Pedidos => Set<Pedido>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pedido>()
                .Property(p => p.ValorTotal)
                .HasPrecision(18, 2);
        }
    }
}
