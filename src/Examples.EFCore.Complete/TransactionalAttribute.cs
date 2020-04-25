using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Examples.EFCore.Complete
{
	/// <summary>
	/// A filter that specifies the controller-action should be wrapped with a <see cref="TransactionScope"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public sealed class TransactionalAttribute : Attribute, IAsyncActionFilter
	{
		/// <summary>
		/// A filter that specifies the controller-action should be wrapped with a <see cref="TransactionScope"/>.
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

			using var transation = new TransactionScope(TransactionScopeOption.Required, _transactionOptions[(int)IsolationLevel], TransactionScopeAsyncFlowOption.Enabled);
			logger.LogInformation("Transaction started for controller-action.");
			var executed = await next();

			if (executed.Exception == null)
			{
				transation.Complete();
				logger.LogInformation("Transaction committed for controller-action.");
			}
			else
				logger.LogInformation("Transaction will be rolled back for controller-action.");
		}

		private static readonly TransactionOptions[] _transactionOptions = new[]
		{
			new TransactionOptions() { IsolationLevel = IsolationLevel.Serializable },
			new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
			new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted },
			new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted },
			new TransactionOptions() { IsolationLevel = IsolationLevel.Snapshot },
			new TransactionOptions() { IsolationLevel = IsolationLevel.Chaos },
			new TransactionOptions() { IsolationLevel = IsolationLevel.Unspecified },
		};
	}
}
