using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient
{
    internal class Order
    {
        public OrderType Type { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
