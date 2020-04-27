using System.Globalization;
using System.Linq;
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
			var anyChanges = false;

			if (await context.Users.AnyAsync() == false)
			{
				anyChanges = true;

				// pulling random users
				using var client = new System.Net.Http.HttpClient();
				var resultStream = await client.GetStreamAsync(new System.Uri("https://randomuser.me/api/?results=100&inc=name,email&nat=us"));
				var jsonUsers = await System.Text.Json.JsonDocument.ParseAsync(resultStream);

				var users =
					from user in jsonUsers.RootElement.GetProperty("results").EnumerateArray()
					let name = user.GetProperty("name")
					select new Data.User()
					{
						FirstName = name.GetProperty("first").GetString(),
						LastName = name.GetProperty("last").GetString(),
						Email = user.GetProperty("email").GetString(),
					};

				context.Users.AddRange(users);
			}

			if (anyChanges)
				await context.SaveChangesAsync();
		}
	}
}