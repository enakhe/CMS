using CMS.Domain.Entities;
using CMS.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CMS.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }

        public DbSet<Criminal> Criminals { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CriminalConfiguration());
        }
    }
}
