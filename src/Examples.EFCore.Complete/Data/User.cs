using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
		/// <summary>If the user should be shown in the results.</summary>
		/// <remarks>Users that are not visible should never be returned out of the service.</remarks>
		public bool Visible { get; set; } = true;
	}
}
