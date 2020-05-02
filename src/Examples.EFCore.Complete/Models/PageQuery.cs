using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Examples.EFCore.Complete.Models
{
	/// <summary>
	/// Allows for representing a requested paged list.
	/// </summary>
	public sealed class PageQuery
	{
		/// <summary>How many results, maximum, were requested for the current page.</summary>
		[DefaultValue(10), Range(0, 5_000)]
		public int Limit { get; set; } = 10;

		/// <summary>Zero-based offset, from the beginning of the list, of the current page.</summary>
		[DefaultValue(0), Range(0, 5_000)]
		public int Offset { get; set; } = 0;

		/// <summary>Sorting order of results. Default order, if not specified.</summary>
		[DefaultValue(null), RegularExpression(@"\w+(?:,\w+)*", ErrorMessage = "Expected format either 'fieldName' or 'fieldName1,fieldName2,...'")]
		public string? OrderBy { get; set; }

		/// <summary>What fields to include in the results. All fields, if not specified.</summary>
		[DefaultValue(null), RegularExpression(@"\w+(?:,\w+)*", ErrorMessage = "Expected format either 'fieldName' or 'fieldName1,fieldName2,...'")]
		public string? Fields { get; set; }
	}
}