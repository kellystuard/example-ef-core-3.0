using System;

namespace Examples.EFCore.DIY.Data
{
	public sealed class UserLogin
	{
		public int Id { get; set; }

		public User User { get; set; }

		public DateTime Timestamp { get; set; } = DateTime.UtcNow;

		public bool Successful { get; set; }
	}
}