using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Examples.EFCore.Complete.Models
{
    public struct Page<T>
    {
        public IEnumerable<T> Results { get; set; }
        public int TotalCount { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}