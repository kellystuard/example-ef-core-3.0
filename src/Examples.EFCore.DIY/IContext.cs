using Microsoft.EntityFrameworkCore;

namespace Examples.EFCore.DIY
{
    public interface IContext
    {
        DbSet<Models.User> Users { get; }
        DbSet<Models.Order> Orders { get; }
    }
}