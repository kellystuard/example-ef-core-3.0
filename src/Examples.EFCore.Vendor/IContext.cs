using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Examples.EFCore.Vendor
{
    public interface IContext
    {
        DbSet<Models.User> Users { get; }
        DbSet<Models.Order> Orders { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}