using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Examples.EFCore.Complete
{
	/// <inheritdoc cref="Context"/>
	public interface IContext
	{
		/// <inheritdoc cref="Context.Users"/>
		DbSet<Data.User> Users { get; }

		/// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)"/>
		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
