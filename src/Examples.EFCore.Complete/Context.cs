using Microsoft.EntityFrameworkCore;

namespace Examples.EFCore.Complete
{
    public sealed class Context : DbContext
    {
        public Context (DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Order> Orders { get; set; }
    }
}