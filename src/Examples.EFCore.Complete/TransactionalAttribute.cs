using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Examples.EFCore.Complete
{
	/// <summary>
	/// A filter that specifies the controller-action should be wrapped with a database transaction.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public sealed class TransactionalAttribute : Attribute, IAsyncActionFilter
	{
		/// <summary>
		/// A filter that specifies the controller-action should be wrapped with a database transaction.
		/// </summary>
		/// <param name="isolationLevel">Specifies the isolation level of a transaction.</param>
		public TransactionalAttribute(IsolationLevel isolationLevel)
		{
			IsolationLevel = isolationLevel;
		}

		/// <summary>Specifies the isolation level of a transaction.</summary>
		public IsolationLevel IsolationLevel { get; }

		/// <inheritdoc/>
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			if (next == null)
				throw new ArgumentNullException(nameof(next));

			var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<TransactionalAttribute>>();
			var db = (Context)context.HttpContext.RequestServices.GetRequiredService<IContext>();

			using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel);
			logger.LogInformation("Transaction started for controller-action.");

			var executed = await next();
			if (executed.Exception == null)
			{
				await transaction.CommitAsync();
				logger.LogInformation("Transaction committed for controller-action.");
			}
			else
			{
				await transaction.RollbackAsync();
				logger.LogInformation("Transaction will be rolled back for controller-action.");
			}
		}
	}
}