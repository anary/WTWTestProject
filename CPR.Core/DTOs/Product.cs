using System.Collections.Generic;

namespace CPR.Core.DTOs
{
    public class Product
    {
        public string Name { get; set; }
        public IDictionary<int, decimal> Values { get; set; }
    }
}