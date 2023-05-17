using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient
{
    internal class OrderBookNaive
    {
        private SortedDictionary<decimal, List<Order>> bids;
        private SortedDictionary<decimal, List<Order>> asks;

        public OrderBookNaive()
        {
            bids = new SortedDictionary<decimal, List<Order>>(Comparer<decimal>.Create((x, y) => y.CompareTo(x)));
            asks = new SortedDictionary<decimal, List<Order>>();
        }

        public void AddOrder(Order order)
        {
            SortedDictionary<decimal, List<Order>> book = order.Type == OrderType.Buy ? bids : asks;

            if (!book.ContainsKey(order.Price))
                book[order.Price] = new List<Order>();

            book[order.Price].Add(order);
        }

        public void RemoveOrder(Order order)
        {
            SortedDictionary<decimal, List<Order>> book = order.Type == OrderType.Buy ? bids : asks;

            if (book.ContainsKey(order.Price))
            {
                book[order.Price].Remove(order);

                if (book[order.Price].Count == 0)
                    book.Remove(order.Price);
            }
        }

        public List<Order> GetBids()
        {
            return bids.Values.SelectMany(orders => orders).ToList();
        }

        public List<Order> GetAsks()
        {
            return asks.Values.SelectMany(orders => orders).ToList();
        }
    }
}
