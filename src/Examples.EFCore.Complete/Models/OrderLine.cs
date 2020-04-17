namespace Examples.EFCore.Complete.Models
{
    public sealed class OrderLine
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
    }
}