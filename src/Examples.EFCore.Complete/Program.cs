using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
					new Models.User() { FirstName = "Sterling", LastName = "Archer", },
					new Models.User() { FirstName = "Cheryl", LastName = "Tunt", },
					new Models.User() { FirstName = "Pam", LastName = "Poovey", },
					new Models.User() { FirstName = "Cyril", LastName = "Figgis", },
					new Models.User() { FirstName = "Lana", LastName = "Kane", },
					new Models.User() { FirstName = "Malory", LastName = "Archer", },
					new Models.User() { FirstName = "Ray", LastName = "Gillette", },
					new Models.User() { FirstName = "Doctor", LastName = "Kreiger", },
					new Models.User() { FirstName = "Barry", LastName = "Dillon", },
					new Models.User() { FirstName = "Other Barry", LastName = "Dillon", },
					new Models.User() { FirstName = "Len", LastName = "Drexler", },
					new Models.User() { FirstName = "Ron", LastName = "Cadillac", },
					new Models.User() { FirstName = "Brett", LastName = "Buckley", },
					new Models.User() { FirstName = "Katya", LastName = "Kazanova", },
					new Models.User() { FirstName = "Gustavo", LastName = "Calderon", }
				);

			await context.SaveChangesAsync();
		}
	}
}
