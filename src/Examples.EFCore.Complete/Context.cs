using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Examples.EFCore.Complete
{
	/// <inheritdoc/>
	public sealed class Context : DbContext, IContext
	{
		private readonly IConfiguration _configuration;

		/// <inheritdoc/>
		public Context(DbContextOptions<Context> options, IConfiguration configuration)
			: base(options)
		{
			_configuration = configuration;
		}

		/// <inheritdoc/>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			if (modelBuilder == null)
				throw new ArgumentNullException(nameof(modelBuilder));

			if (_configuration.GetValue("showInvisibleUsers", false) == false)
				modelBuilder.Entity<Data.User>().HasQueryFilter(u => u.Visible);
		}

		/// <summary>
		/// Deferred full list of users, from the database.
		/// </summary>
		public DbSet<Data.User> Users { get; set; } = null!;
	}
}