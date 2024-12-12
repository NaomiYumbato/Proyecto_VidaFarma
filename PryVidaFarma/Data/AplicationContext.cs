using Microsoft.EntityFrameworkCore;
using PryVidaFarma.Models;

namespace PryVidaFarma.Data
{
    public class AplicationContext : DbContext
    { 
        
        public AplicationContext(DbContextOptions<AplicationContext> options) : 
                base(options)
        { 

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categorias>().HasKey(c => c.id_categoria);
            modelBuilder.Entity<Productos>().HasKey(p => p.id_producto);
        }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Categorias> Categorias { get; set; }
    }
}
