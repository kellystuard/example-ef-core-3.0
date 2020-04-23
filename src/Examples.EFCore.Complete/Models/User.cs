using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Examples.EFCore.Complete.Models
{
	/// <summary>
	/// User in the database.
	/// </summary>
	public sealed class User
	{
		/// <summary>User's database identifier.</summary>
		public int Id { get; set; }
		/// <summary>User's first name.</summary>
		[Required, Display(Name = "First Name")]
		public string? FirstName { get; set; }
		/// <summary>User's last name.</summary>
		[Required, Display(Name = "Last Name")]
		public string? LastName { get; set; }
		/// <summary>If the user should be shown in the results.</summary>
		/// <remarks>Users that are not visible should never be returned out of the service.</remarks>
		[JsonIgnore]
		public bool Visible { get; set; } = true;
	}
}
