using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient.Models
{
    internal class GeminiWebSocketEvent
    {
        public string type { get; set; }
        public string side { get; set; }
        public decimal price { get; set; }
        public decimal remaining { get; set; }
        public decimal delta { get; set; }
        public string reason { get; set; }

        public long tid { get; set; }
        public decimal amount { get; set; }
        public string makerSide { get; set; }

        public override string ToString()
        {
            return $"[type={type}, side={side}, price={price}, remaining={remaining}, delta={delta}, reason={reason}]";
        }
    }
}
