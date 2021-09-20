using MotorbikeSpecs.Model;
using Microsoft.EntityFrameworkCore;

namespace MotorbikeSpecs.Data
{
    public class BraapDbContext : DbContext
    {
        public BraapDbContext(DbContextOptions options) : base(options) { }

        public DbSet<BraapUser> BraapUsers { get; set; } = default!;
        public DbSet<Motorbike> Motorbikes { get; set; } = default!;
        public DbSet<Company> Companies { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Motorbike>()
                .HasOne(p => p.Company)
                .WithMany(s => s.Motorbikes)
                .HasForeignKey(p => p.CompanyId);

            modelBuilder.Entity<Review>()
                .HasOne(c => c.BraapUser)
                .WithMany(s => s.Reviews)
                .HasForeignKey(c => c.BraapUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(c => c.Motorbike)
                .WithMany(p => p.Reviews)
                .HasForeignKey(c => c.MotorbikeId);
        }
    }
}
