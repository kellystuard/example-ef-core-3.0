using Microsoft.Extensions.DependencyInjection;

namespace Examples.EFCore.Complete
{
	/// <inheritdoc/>
	public sealed class StartupDesign : Microsoft.EntityFrameworkCore.Design.IDesignTimeServices
	{
		/// <inheritdoc/>
		public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<Microsoft.EntityFrameworkCore.Migrations.Design.IMigrationsCodeGenerator, Migrations.CS1591MigrationsGenerator>();
		}
	}
}
