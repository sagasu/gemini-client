using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient
{
    internal class OrderBookNaive
    {
        private readonly SortedDictionary<decimal, List<Order>> _bids;
        private readonly SortedDictionary<decimal, List<Order>> _asks;

        public OrderBookNaive()
        {
            _bids = new SortedDictionary<decimal, List<Order>>(Comparer<decimal>.Create((x, y) => y.CompareTo(x)));
            _asks = new SortedDictionary<decimal, List<Order>>();
        }

        public void AddOrder(Order order)
        {
            var book = order.Type == OrderType.Buy ? _bids : _asks;

            if (!book.ContainsKey(order.Price))
                book[order.Price] = new List<Order>();

            book[order.Price].Add(order);
        }

        public void RemoveOrder(Order order)
        {
            var book = order.Type == OrderType.Buy ? _bids : _asks;

            if (book.ContainsKey(order.Price))
            {
                book[order.Price].Remove(order);

                if (book[order.Price].Count == 0)
                    book.Remove(order.Price);
            }
        }

        public List<Order> GetBids() => _bids.Values.SelectMany(orders => orders).ToList();
        
        public List<Order> GetAsks() =>_asks.Values.SelectMany(orders => orders).ToList();
        
    }
}
