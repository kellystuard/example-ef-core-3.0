using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
		/// <param name="environment">Provides information about the web hosting environment an application is running in.</param>
		public Startup(IConfiguration configuration, IWebHostEnvironment environment)
		{
			Configuration = configuration;
			Environment = environment;
		}

		/// <summary>Represents a set of key/value application configuration properties.</summary>
		public IConfiguration Configuration { get; }

		/// <summary>Provides information about the web hosting environment an application is running in.</summary>
		public IWebHostEnvironment Environment { get; }

		/// <summary>
		/// Configures the services, during the application startup.
		/// </summary>
		/// <param name="services">Specifies the contract for a collection of service descriptors.</param>
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<Context>(options => options
				.UseSqlServer(Configuration.GetConnectionString("Database"))
				.EnableSensitiveDataLogging(Environment.IsDevelopment())
			);
			services.AddControllers(configure =>
			{
				configure.Filters.Add<OperationCancelledExceptionFilterAttribute>();
			})
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
					options.JsonSerializerOptions.IgnoreNullValues = true;
				});

			services.AddScoped<IContext, Context>();
			services.AddTransient(typeof(Lazy<>), typeof(ServiceLazy<>));
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Awesome User API", Version = "v1" });

				// Set the comments path for the Swagger JSON and UI.
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});
			services.AddAutoMapper(typeof(Startup));
			services.AddHealthChecks()
				.AddSqlServer(Configuration.GetConnectionString("Database"))
				.AddDiskStorageHealthCheck(options => options.AddDrive("C:\\", 1000), "Disk low available storage - Degraded", HealthStatus.Degraded)
				.AddDiskStorageHealthCheck(options => options.AddDrive("C:\\", 100), "Disk low available storage - Unhealthy", HealthStatus.Unhealthy)
			;
		}

		/// <summary>
		/// Configures the application, during application startup.
		/// </summary>
		/// <param name="app">Provides the mechanisms to configure an application's request pipeline.</param>
		public void Configure(IApplicationBuilder app)
		{
			if (Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

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
				endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
				{
					ResponseWriter = WriteResponse,
				});
			});
		}

		private static Task WriteResponse(HttpContext context, HealthReport result)
		{
			context.Response.ContentType = "application/json; charset=utf-8";

			using var writer = new Utf8JsonWriter(context.Response.BodyWriter);
			writer.WriteStartObject();
			writer.WriteString("status", result.Status.ToString());
			writer.WriteStartObject("results");
			foreach (var entry in result.Entries)
			{
				writer.WriteStartObject(entry.Key);
				writer.WriteString("status", entry.Value.Status.ToString());
				writer.WriteString("description", entry.Value.Description);
				writer.WriteStartObject("data");
				foreach (var item in entry.Value.Data)
				{
					writer.WritePropertyName(item.Key);
					JsonSerializer.Serialize(
						writer, item.Value, item.Value?.GetType() ??
						typeof(object));
				}
				writer.WriteEndObject();
				writer.WriteEndObject();
			}
			writer.WriteEndObject();
			writer.WriteEndObject();

			return writer.FlushAsync();
		}
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used in DI registrations to allow for lazy resolving of dependencies.")]
	internal sealed class ServiceLazy<T> : Lazy<T> where T : class
	{
		public ServiceLazy(IServiceProvider provider)
			: base(() => provider.GetRequiredService<T>())
		{
		}
	}
}