using System.Collections.Generic;

namespace Examples.EFCore.DIY.Data
{
	public sealed class User
	{
		public int Id { get; set; }

		public string FirstName { get; set; } = null!;

		public string LastName { get; set; } = null!;

		public string Email { get; set; } = null!;

		public bool Visible { get; set; } = true;

		public List<UserLogin> Logins { get; } = new List<UserLogin>();
	}
}