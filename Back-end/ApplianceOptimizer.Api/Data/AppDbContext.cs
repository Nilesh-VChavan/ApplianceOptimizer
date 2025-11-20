using ApplianceOptimizer.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplianceOptimizer.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Appliance> Appliances { get; set; }
    }
}
