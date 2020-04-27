using System.ComponentModel.DataAnnotations;

namespace Examples.EFCore.Complete.Models
{
	/// <summary>
	/// User in the API.
	/// </summary>
	public sealed class User
	{
		/// <summary>User's API identifier.</summary>
		public int Id { get; set; }

		/// <summary>User's first name.</summary>
		[Required, Display(ShortName = "firstName", Name = "First Name")]
		public string FirstName { get; set; } = null!;

		/// <summary>User's last name.</summary>
		[Required, Display(ShortName = "lastName", Name = "Last Name")]
		public string LastName { get; set; } = null!;

		/// <summary>User's last name.</summary>
		[Required, Display(ShortName = "email", Name = "Email")]
		public string Email { get; set; } = null!;

		/// <summary>
		/// Controls the sort-order for users.
		/// </summary>
		public enum Sort
		{
			/// <summary>Sorts by last name then first name.</summary>
			Default = 0,

			/// <summary>Sorts by database identifier.</summary>
			Id,

			/// <summary>Sorts by first name then last name.</summary>
			FirstName,

			/// <summary>Sorts by last name then first name.</summary>
			LastName,

			/// <summary>Sorts by email, last name, then first name.</summary>
			Email,
		}
	}
}