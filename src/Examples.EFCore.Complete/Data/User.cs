using System.Collections.Generic;

namespace Examples.EFCore.Complete.Data
{
	/// <summary>
	/// User in the database.
	/// </summary>
	public sealed class User
	{
		/// <summary>User's database identifier.</summary>
		public int Id { get; set; }

		/// <summary>User's first name.</summary>
		public string FirstName { get; set; } = null!;

		/// <summary>User's last name.</summary>
		public string LastName { get; set; } = null!;

		/// <summary>User's last name.</summary>
		public string Email { get; set; } = null!;

		/// <summary>If the user should be shown in the results.</summary>
		/// <remarks>Users that are not visible should never be returned out of the service.</remarks>
		public bool Visible { get; set; } = true;

		/// <summary>User's list of logins.</summary>
		public List<UserLogin> Logins { get; } = new List<UserLogin>();
	}
}