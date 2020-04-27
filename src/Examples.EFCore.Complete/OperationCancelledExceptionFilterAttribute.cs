using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Examples.EFCore.Complete
{
	/// <summary>
	/// Converts <see cref="OperationCanceledException"/> in to HTTP status code result of 499 (Client Closed Request).
	/// </summary>
	/// <remarks>
	/// Used to keep API client canceling request from returning HTTP status code of 500. Canceled requests trigger
	/// injected <see cref="CancellationToken"/>s to signal cancellation. This causes the exception to be thrown.
	/// </remarks>
	public sealed class OperationCancelledExceptionFilterAttribute : ExceptionFilterAttribute
	{
		private readonly int _statusCode;

		/// <summary>
		/// Creates an instance of the class.
		/// </summary>
		/// <param name="statusCode">HTTP status code to return. Defaults to  499 (Client Closed Request).</param>
		public OperationCancelledExceptionFilterAttribute(int statusCode = 499)
		{
			_statusCode = statusCode;
		}

		/// <inheritdoc/>
		public override void OnException(ExceptionContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (context?.Exception is OperationCanceledException)
			{
				var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<OperationCancelledExceptionFilterAttribute>>();
				logger.LogInformation("OperationCancelledException turned in to HTTP result of {StatusCode}. Assuming user canceled request.", _statusCode);
				context.ExceptionHandled = true;
				context.Result = new StatusCodeResult(_statusCode);
			}
		}
	}
}