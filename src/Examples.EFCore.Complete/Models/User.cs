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

		/// <summary>Number of times the user has logged in.</summary>
		[Display(ShortName = "loginCount", Name = "Login Count")]
		public int LoginCount { get; set; }
	}
}