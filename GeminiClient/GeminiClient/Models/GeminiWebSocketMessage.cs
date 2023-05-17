using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient.Models
{
    internal class GeminiWebSocketMessage
    {
        public string type { get; set; }
        public long eventId { get; set; }
        public int timestamp { get; set; }
        public long timestampms { get; set; }
        public int socket_sequence { get; set; }
        public List<GeminiWebSocketEvent> events { get; set; }

        public override string ToString()
        {
            return events.ToString();
        }
    }
}
