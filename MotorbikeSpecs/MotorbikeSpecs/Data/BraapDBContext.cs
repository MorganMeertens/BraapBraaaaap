using MotorbikeSpecs.Model;
using Microsoft.EntityFrameworkCore;

namespace MotorbikeSpecs.Data
{
    public class BraapDbContext : DbContext
    {
        public BraapDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
