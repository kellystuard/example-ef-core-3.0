using System;
using System.Collections.Generic;

namespace Examples.EFCore.Complete.Models
{
	/// <summary>
	/// Allows for representing a paged list of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">Type of list to represent a page of.</typeparam>
	public readonly struct Page<T> : IEquatable<Page<T>>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="results">Current page list.</param>
		/// <param name="totalCount">Total count of the full list.</param>
		/// <param name="limit">How many results, maximum, were requested for the current page.</param>
		/// <param name="offset">Zero-based offset, from the beginning of the list, of the current page.</param>
		public Page(IEnumerable<T> results, int totalCount, int limit, int offset)
		{
			Results = results;
			TotalCount = totalCount;
			Limit = limit;
			Offset = offset;
		}

		/// <summary>Current page list.</summary>
		public IEnumerable<T> Results { get; }
		/// <summary>Total count of the full list.</summary>
		public int TotalCount { get; }
		/// <summary>How many results, maximum, were requested for the current page.</summary>
		public int Limit { get; }
		/// <summary>Zero-based offset, from the beginning of the list, of the current page.</summary>
		public int Offset { get; }

		#region Equality methods and operators

		/// <inheritdoc/>
		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(this, obj))
				return true;

			return (obj is Page<T> right && Equals(right));
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return HashCode.Combine(Limit, Offset, TotalCount);
		}

		/// <inheritdoc/>
		public static bool operator ==(Page<T> left, Page<T> right)
		{
			return left.Equals(right);
		}

		/// <inheritdoc/>
		public static bool operator !=(Page<T> left, Page<T> right)
		{
			return !left.Equals(right);
		}

		/// <inheritdoc/>
		public bool Equals(Page<T> other)
		{
			if (object.ReferenceEquals(this, other))
				return true;

			return Limit == other.Limit
				&& Offset == other.Offset
				&& TotalCount == other.TotalCount;
		}

		#endregion
	}
}
