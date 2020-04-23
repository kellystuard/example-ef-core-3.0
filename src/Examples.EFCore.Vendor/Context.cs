using Microsoft.EntityFrameworkCore;

namespace Examples.EFCore.Vendor
{
    public sealed class Context : DbContext, IContext
    {
        public Context (DbContextOptions<Context> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.User>().HasQueryFilter(u => u.Visible);
        }

        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Order> Orders { get; set; }
    }
}