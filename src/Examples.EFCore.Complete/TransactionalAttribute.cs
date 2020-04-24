using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Examples.EFCore.Complete
{
	/// <summary>
	/// A filter that specifies the controller-action should be wrapped with a <see cref="TransactionScope"/>.
	/// </summary>
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

		async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			using var transation = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() 
				{
					IsolationLevel = IsolationLevel.ReadCommitted
				}, TransactionScopeAsyncFlowOption.Enabled);

			var executed = await next();

			if(executed.Exception == null)
				transation.Complete();
		}
	}
}
