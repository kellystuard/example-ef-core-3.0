using System;

namespace Examples.EFCore.Complete.Data
{
	/// <summary>
	/// User historical logins in the database.
	/// </summary>
	public sealed class UserLogin
	{
		/// <summary>User login database identifier.</summary>
		public int Id { get; set; }

		/// <summary>UTC time-stamp that marks the user's login.</summary>
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;

		/// <summary>If the user's login was successful.</summary>
		public bool Successful { get; set; }
	}
}