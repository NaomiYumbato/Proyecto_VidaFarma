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
        public DbSet<Productos> Productos { get; set; }
    }
}
