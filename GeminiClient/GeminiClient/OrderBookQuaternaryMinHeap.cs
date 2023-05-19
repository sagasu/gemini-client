using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient
{
    internal class OrderBookQuaternaryMinHeap
    {
        // PriorityQueue is a Quaternary Min Heap, because it is quaternary it should be faster than binary
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.priorityqueue-2?view=net-6.0#remarks
        // search: O(n) 
        // insert, removal: O(log n), but on average insert is O(1)
        // get min: O(1) 
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Collections/src/System/Collections/Generic/PriorityQueue.cs

        private readonly PriorityQueue<decimal, decimal> _bids;
        private readonly PriorityQueue<decimal, decimal> _asks;

        public OrderBookQuaternaryMinHeap()
        {
            _bids = new PriorityQueue<decimal, decimal>(Comparer<decimal>.Create((x, y) => y.CompareTo(x)));
            _asks = new PriorityQueue<decimal, decimal>();
        }

        public void AddOrder(Order order)
        {
            var book = order.Type == OrderType.Buy ? _bids : _asks;

            if (!book.ContainsKey(order.Price))
                book[order.Price] = 0;

            book[order.Price] += order.Quantity;
        }

        public void RemoveOrder(Order order)
        {
            var book = order.Type == OrderType.Buy ? _bids : _asks;

            if (book.ContainsKey(order.Price))
            {
                book[order.Price] -= order.Quantity;

                if (book[order.Price] == 0)
                    book.Remove(order.Price);
            }
        }

        public KeyValuePair<decimal, decimal> GetBestBid() => _bids.Peek();

        public KeyValuePair<decimal, decimal> GetBestAsk() => _asks.Peek();

    }
}
