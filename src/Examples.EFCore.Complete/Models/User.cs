namespace Examples.EFCore.Complete.Models
{
    public sealed class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Visible { get; set; } = true;
    }
}