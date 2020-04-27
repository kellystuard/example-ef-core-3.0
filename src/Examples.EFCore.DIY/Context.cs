using Microsoft.EntityFrameworkCore;

namespace Examples.EFCore.DIY
{
	public sealed class Context : DbContext
	{
		public Context(DbContextOptions<Context> options)
			: base(options)
		{
		}

		public DbSet<Data.User> Users { get; set; }
		public DbSet<Data.UserLogin> UserLogins { get; set; }
	}
}