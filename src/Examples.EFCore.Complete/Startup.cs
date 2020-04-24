using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Examples.EFCore.Complete
{
	/// <summary>
	/// Called as part of application startup, to setup the application and services.
	/// </summary>
	public sealed class Startup
	{
		/// <summary>
		/// Creates a new instance with a specified configuration.
		/// </summary>
		/// <param name="configuration">Represents a set of key/value application configuration properties.</param>
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		/// Represents a set of key/value application configuration properties.
		/// </summary>
		public IConfiguration Configuration { get; }

		/// <summary>
		/// Configures the services, during the application startup.
		/// </summary>
		/// <param name="services">Specifies the contract for a collection of service descriptors.</param>
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<Context>(options => options
				.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0")
				.EnableSensitiveDataLogging(true)
			);
			services.AddControllers();
			services.AddScoped<IContext, Context>();
			services.AddTransient(typeof(Lazy<>), typeof(ServiceLazy<>));
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

				// Set the comments path for the Swagger JSON and UI.
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});
		}

		/// <summary>
		/// Configures the application, during application startup.
		/// </summary>
		/// <param name="app">Provides the mechanisms to configure an application's request pipeline.</param>
		/// <param name="env">Provides information about the web hosting environment an application is running in.</param>
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Startup).Namespace);
			});

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		internal sealed class ServiceLazy<T> : Lazy<T> where T : class
		{
			public ServiceLazy(IServiceProvider provider)
				: base(() => provider.GetRequiredService<T>())
			{
			}
		}
	}
}
