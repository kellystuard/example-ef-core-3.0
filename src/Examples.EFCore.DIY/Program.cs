using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Examples.EFCore.DIY
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
			using var scope = host.Services.CreateScope();
			var services = scope.ServiceProvider;

			var context = services.GetRequiredService<Context>();
			await CreateDbIfNotExists(context);
			await PopulateDbIfEmpty(context, Get100RandomUsers);

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

		/// <summary>
		/// Creates the database and runs migrations, if it does not already exist.
		/// </summary>
		/// <param name="context">A DbContext instance represents a session with the database and can be used to query and save instances of your entities.</param>
		/// <returns></returns>
		public static async Task CreateDbIfNotExists(Context context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			await context.Database.MigrateAsync();
		}

		/// <summary>
		/// Populates the database, if the tables are empty.
		/// </summary>
		/// <param name="context">A DbContext instance represents a session with the database and can be used to query and save instances of your entities.</param>
		/// <param name="users">Function that, when executed, returns the users to load.</param>
		/// <returns></returns>
		public static async Task PopulateDbIfEmpty(Context context, Func<Task<IEnumerable<Data.User>>> users)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			if (users == null)
				throw new ArgumentNullException(nameof(users));

			if (await context.Users.AnyAsync() == false)
			{
				context.Users.AddRange(await users());
				await context.SaveChangesAsync();
			}
		}

		/// <summary>
		/// Converts 100 users from randomuser.me in to <see cref="Data.User"/>s.
		/// </summary>
		/// <returns>100 users from randomuser.me.</returns>
		public static async Task<IEnumerable<Data.User>> Get100RandomUsers()
		{
			var minMinutes = (int)TimeSpan.FromDays(1).TotalMinutes;
			var maxMinutes = (int)TimeSpan.FromDays(365).TotalMinutes;

			// pulling random users
			// using the same seed gives the same results every time
			using var client = new System.Net.Http.HttpClient();
			var resultStream = await client.GetStreamAsync(new Uri($"https://randomuser.me/api/?results=100&inc=name,email&nat=us&seed={maxMinutes}"));
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

			// using the same seed gives the same results every time
			var random = new Random(maxMinutes);
			var now = DateTime.UtcNow;

			// setup each random user with between 0 and 99 historic logins
			foreach (var user in users)
			{
				var lnow = now;
				var logins = new Data.UserLogin[random.Next(100)];
				// with each login between 1 day and 1 year ago
				for (int i = 0; i < logins.Length; i++)
				{
					lnow -= TimeSpan.FromMinutes(random.Next(minMinutes, maxMinutes));
					logins[i] = new Data.UserLogin()
					{
						Successful = random.Next(0, 1) == 1,
						Timestamp = lnow,
					};
				}
				user.Logins.AddRange(logins.Reverse());
			}

			return users;
		}
	}
}