using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Examples.EFCore.Complete
{
	/// <summary>
	/// Converts <see cref="OperationCanceledException"/> in to HTTP status code result of 499 (Client Closed Request).
	/// </summary>
	/// <remarks>
	/// Used to keep API client canceling request from returning HTTP status code of 500.
	/// </remarks>
	public sealed class OperationCancelledExceptionFilterAttribute : ExceptionFilterAttribute
	{
		private readonly ILogger _logger;

		/// <summary>
		/// Creates an instance of the class.
		/// </summary>
		/// <param name="logger">Represents a type used to perform logging.</param>
		public OperationCancelledExceptionFilterAttribute(ILogger<OperationCancelledExceptionFilterAttribute> logger)
		{
			_logger = logger;
		}

		/// <inheritdoc/>
		public override void OnException(ExceptionContext context)
		{
			if (context?.Exception is OperationCanceledException)
			{
				_logger.LogInformation("User canceled request.");
				context.ExceptionHandled = true;
				context.Result = new StatusCodeResult(499); // Client Closed Request
			}
		}
	}
}
