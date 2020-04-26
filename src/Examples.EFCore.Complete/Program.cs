using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Examples.EFCore.Complete
{
	/// <summary>
	/// Starting class of the application.
	/// </summary>
	public sealed class Program
	{
		/// <summary>
		/// Starting method of the application.
		/// </summary>
		/// <param name="args">Command line arguments passed to the application.</param>
		public static async Task Main(string[] args)
		{
			System.Diagnostics.Activity.DefaultIdFormat = System.Diagnostics.ActivityIdFormat.W3C;

			var host = CreateHostBuilder(args).Build();
			await CreateAndPopulateDbIfNotExists(host);
			await host.RunAsync();
		}

		/// <summary>
		/// Creates the web-host that will run the service endpoints.
		/// </summary>
		/// <param name="args">Command line arguments passed to the application.</param>
		/// <returns></returns>
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

		private static async Task CreateAndPopulateDbIfNotExists(IHost host)
		{
			using var scope = host.Services.CreateScope();
			var services = scope.ServiceProvider;

			var context = services.GetRequiredService<Context>();
			context.Database.Migrate();

			if (await context.Users.AnyAsync() == false)
				context.Users.AddRange(
					new Data.User() { FirstName = "Sterling", LastName = "Archer", },
					new Data.User() { FirstName = "Cheryl", LastName = "Tunt", },
					new Data.User() { FirstName = "Pam", LastName = "Poovey", },
					new Data.User() { FirstName = "Cyril", LastName = "Figgis", },
					new Data.User() { FirstName = "Lana", LastName = "Kane", },
					new Data.User() { FirstName = "Malory", LastName = "Archer", },
					new Data.User() { FirstName = "Ray", LastName = "Gillette", },
					new Data.User() { FirstName = "Doctor", LastName = "Kreiger", },
					new Data.User() { FirstName = "Barry", LastName = "Dillon", },
					new Data.User() { FirstName = "Other Barry", LastName = "Dillon", },
					new Data.User() { FirstName = "Len", LastName = "Drexler", },
					new Data.User() { FirstName = "Ron", LastName = "Cadillac", },
					new Data.User() { FirstName = "Brett", LastName = "Buckley", },
					new Data.User() { FirstName = "Katya", LastName = "Kazanova", },
					new Data.User() { FirstName = "Gustavo", LastName = "Calderon", }
				);

			await context.SaveChangesAsync();
		}
	}
}
