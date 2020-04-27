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

namespace Examples.EFCore.DIY
{
	/// <summary>
	/// Starting class of the application.
	/// </summary>
	public sealed class Program
	{
		/// <summary>Makes data population deterministic.</summary>
		public const int Seed = 525_600;

		/// <summary>
		/// Starting method of the application.
		/// </summary>
		/// <param name="args">Command line arguments passed to the application.</param>
		public static async Task Main(string[] args)
		{
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
				var resultStream = await client.GetStreamAsync(new System.Uri($"https://randomuser.me/api/?results=100&inc=name,email&nat=us&seed={Seed}"));
				var jsonUsers = await System.Text.Json.JsonDocument.ParseAsync(resultStream);

				var users = (
					from user in jsonUsers.RootElement.GetProperty("results").EnumerateArray()
					let name = user.GetProperty("name")
					select new Data.User()
					{
						FirstName = name.GetProperty("first").GetString(),
						LastName = name.GetProperty("last").GetString(),
						Email = user.GetProperty("email").GetString(),
					}
				).ToArray();

				var random = new Random(Seed);
				var now = DateTime.UtcNow;
				var maxMinutes = TimeSpan.FromDays(365).TotalMinutes;

				foreach (var user in users)
				{
					var lnow = now;
					var logins = new Data.UserLogin[random.Next(100)];
					for (int i = 0; i < logins.Length; i++)
					{
						lnow -= TimeSpan.FromMinutes(maxMinutes);
						logins[i] = new Data.UserLogin()
						{
							Successful = random.Next(0, 1) == 1,
							Timestamp = lnow,
						};
					}
					user.Logins.AddRange(logins.Reverse());
				}

				context.Users.AddRange(users);
			}

			if (anyChanges)
				await context.SaveChangesAsync();
		}
	}
}