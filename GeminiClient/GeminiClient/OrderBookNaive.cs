using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient
{
    internal class OrderBookNaive
    {
        // SortedDictionary is a BinaryTree
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2?view=net-7.0#remarks
        // search: O(log n) 
        // insert, removal: O(log n) 
        // unfortunately to get first element it is also a O(log n) https://github.com/dotnet/runtime/issues/18668
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Collections/src/System/Collections/Generic/SortedDictionary.cs

        private readonly SortedDictionary<decimal, decimal> _bids;
        private readonly SortedDictionary<decimal, decimal> _asks;

        private decimal bestBidPrice;
        private decimal bestBidQuantity;
        private decimal bestAskPrice;
        private decimal bestAskQuantity;

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

            book[order.Price] = order.Quantity;
            UpdateBestPriceAndQuantityForBook(order);
        }

        public void RemoveOrder(Order order)
        {
            var book = order.Type == OrderType.Buy ? _bids : _asks;

            if (book.ContainsKey(order.Price))
            {
                book[order.Price] = order.Quantity;

                if (book[order.Price] == 0)
                {
                    book.Remove(order.Price);

                    if (order.Type == OrderType.Buy)
                    {
                        var bestBid = GetBestBid();
                        bestBidPrice = bestBid.Key;
                        bestBidQuantity = bestBid.Value;
                        Console.WriteLine($"{bestBidPrice} {bestBidQuantity} {bestAskPrice} {bestAskQuantity}");
                    }
                    else
                    {
                        var bestBid = GetBestAsk();
                        bestAskPrice = bestBid.Key;
                        bestAskQuantity = bestBid.Value;
                        Console.WriteLine($"{bestBidPrice} {bestBidQuantity} {bestAskPrice} {bestAskQuantity}");
                    }
                }
            }
        }

        void UpdateBestPriceAndQuantityForBook(Order order)
        {
            if (order.Type == OrderType.Buy)
            {
                if (order.Price >= bestBidPrice)
                {
                    bestBidPrice = order.Price;
                    bestBidQuantity = order.Quantity;
                    Console.WriteLine($"{bestBidPrice} {bestBidQuantity} {bestAskPrice} {bestAskQuantity}");
                }
            }
            else
            {
                if (order.Price <= bestAskPrice)
                {
                    bestAskPrice = order.Price;
                    bestAskQuantity = order.Quantity;
                    Console.WriteLine($"{bestBidPrice} {bestBidQuantity} {bestAskPrice} {bestAskQuantity}");
                }
            }
        }

        public KeyValuePair<decimal, decimal> GetBestBid() => _bids.FirstOrDefault();

        public KeyValuePair<decimal, decimal> GetBestAsk() => _asks.FirstOrDefault();

    }
}
