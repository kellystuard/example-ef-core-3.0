using Microsoft.EntityFrameworkCore;

namespace Examples.EFCore.Complete
{
	/// <inheritdoc/>
	public sealed class Context : DbContext, IContext
	{
		/// <inheritdoc/>
		public Context(DbContextOptions<Context> options)
			: base(options)
		{
		}

		/// <inheritdoc/>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Models.User>().HasQueryFilter(u => u.Visible);
		}

		/// <summary>
		/// Deferred full list of users, from the database.
		/// </summary>
		public DbSet<Models.User> Users { get; set; } = null!;
	}
}