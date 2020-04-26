using System.ComponentModel.DataAnnotations;

namespace Examples.EFCore.Complete.Models
{
	/// <summary>
	/// User in the API.
	/// </summary>
	public sealed class UserEdit
	{
		/// <summary>User's first name.</summary>
		[Display(ShortName = "firstName", Name = "First Name")]
		public string FirstName { get; set; } = null!;
		/// <summary>User's last name.</summary>
		[Display(ShortName = "lastName", Name = "Last Name")]
		public string LastName { get; set; } = null!;
	}
}
