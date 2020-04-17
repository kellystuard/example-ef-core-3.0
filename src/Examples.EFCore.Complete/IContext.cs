using Microsoft.EntityFrameworkCore;

namespace Examples.EFCore.Complete
{
    public interface IContext
    {
        DbSet<Models.User> Users { get; }
        DbSet<Models.Order> Orders { get; }

        int SaveChanges();
    }
}