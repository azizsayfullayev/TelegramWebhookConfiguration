using Microsoft.EntityFrameworkCore;
using FastApiWebhook.Models;

namespace FastApiWebhook.DbContexts
{
    public class AppDbContext : DbContext
    {

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Movie> Movies { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}
