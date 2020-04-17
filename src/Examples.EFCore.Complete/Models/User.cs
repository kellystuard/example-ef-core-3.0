using System.ComponentModel.DataAnnotations;

namespace Examples.EFCore.Complete.Models
{
    public sealed class User
    {
        public int Id { get; set; }
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }
        public bool Visible { get; set; } = true;
    }
}