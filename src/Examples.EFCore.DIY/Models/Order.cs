using System.Collections.Generic;

namespace Examples.EFCore.DIY.Models
{
    public sealed class Order
    {
        public int Id { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public List<OrderLine> Lines { get; set; } = new List<OrderLine>();

        public Order AddLine(OrderLine line)
        {
            line.Order = this;
            this.Lines.Add(line);
            return this;
        }
    }
}