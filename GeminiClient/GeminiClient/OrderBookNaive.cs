using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient
{
    internal class OrderBookNaive
    {
        private readonly SortedDictionary<decimal, decimal> _bids;
        private readonly SortedDictionary<decimal, decimal> _asks;

        public OrderBookNaive()
        {
            _bids = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Create((x, y) => y.CompareTo(x)));
            _asks = new SortedDictionary<decimal, decimal>();
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

        public KeyValuePair<decimal, decimal> GetBestBid() => _bids.FirstOrDefault();

        public KeyValuePair<decimal, decimal> GetBestAsk() => _asks.FirstOrDefault();

    }
}
