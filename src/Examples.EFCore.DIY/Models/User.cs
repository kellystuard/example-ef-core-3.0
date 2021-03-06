using System.ComponentModel.DataAnnotations;

namespace Examples.EFCore.DIY.Models
{
	public sealed class User
	{
		public int Id { get; set; }

		public string FirstName { get; set; } = null!;

		public string LastName { get; set; } = null!;

		public string Email { get; set; } = null!;

		public int LoginCount { get; set; }
	}
}