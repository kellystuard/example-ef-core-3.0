using Microsoft.EntityFrameworkCore;

namespace Examples.EFCore.DIY
{
    public sealed class Context : DbContext
    {
        public Context (DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Models.User> Users { get; set; }
    }
}