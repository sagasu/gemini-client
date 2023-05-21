using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient
{
    internal class OrderBookSortedDictionary
    {
        // SortedDictionary is a BinaryTree
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2?view=net-7.0#remarks
        // search: O(log n) 
        // insert, removal: O(log n) 
        // unfortunately to get first element it is also a O(log n) https://github.com/dotnet/runtime/issues/18668
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Collections/src/System/Collections/Generic/SortedDictionary.cs

        internal SortedDictionary<decimal, decimal> Bids { get; private set; }
        internal SortedDictionary<decimal, decimal> Asks { get; private set; }

        internal decimal? BestBidPrice { get; private set; }
        internal decimal? BestBidQuantity { get; private set; }
        internal decimal? BestAskPrice { get; private set; }
        internal decimal? BestAskQuantity { get; private set; }

        public event EventHandler PriceChangedEmitter;

        public OrderBookSortedDictionary()
        {
            Bids = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Create((x, y) => y.CompareTo(x)));
            Asks = new SortedDictionary<decimal, decimal>();
        }

        public void AddOrder(Order order)
        {
            var book = order.Type == OrderType.Buy ? Bids : Asks;

            book[order.Price] = order.Quantity;
            CacheBestPrice(order, book);
        }

        public void RemoveOrder(Order order)
        {
            var book = order.Type == OrderType.Buy ? Bids : Asks;

            if (book.ContainsKey(order.Price))
            {
                book[order.Price] = order.Quantity;

                if (book[order.Price] == 0)
                {
                    book.Remove(order.Price);
                    CacheBestPriceAfterRemoval(order, book);

                }
            }
        }

        private void CacheBestPriceAfterRemoval(Order order, SortedDictionary<decimal, decimal> book)
        {
            if (order.Type == OrderType.Buy)
            {
                if (order.Price == BestBidPrice)
                {
                    (BestBidPrice, BestBidQuantity) = GetBestPriceFromBook(book);
                    EmitPriceChanged();
                }
            }
            else
            {
                if (order.Price == BestAskPrice)
                {
                    (BestAskPrice, BestAskQuantity) = GetBestPriceFromBook(book);
                    EmitPriceChanged();
                }
            }
        }

        private void EmitPriceChanged() => PriceChangedEmitter.Invoke(this, EventArgs.Empty);
        

        private (decimal?, decimal?) GetBestPriceFromBook(SortedDictionary<decimal, decimal> book)
        {
            if (book.Count == 0) return (null, null);
            
            Console.WriteLine($"hitting best bid/ask");
            var bestBid = book.FirstOrDefault();
            return (bestBid.Key, bestBid.Value);
        }

        //private void PrintState() => Console.WriteLine($"bp:{BestBidPrice} bq:{BestBidQuantity} ap:{BestAskPrice} aq:{BestAskQuantity} asks:{Asks.Count} bids:{Bids.Count}");
        
        void CacheBestPrice(Order order, SortedDictionary<decimal, decimal> book)
        {
            if (order.Type == OrderType.Buy)
            {
                if (order.Price >= BestBidPrice || BestBidPrice is null)
                {
                    (BestBidPrice, BestBidQuantity) = GetBestPrice(order, book);
                    EmitPriceChanged();
                }
            }
            else
            {
                if (order.Price <= BestAskPrice || BestAskPrice is null)
                {
                    (BestAskPrice, BestAskQuantity) = GetBestPrice(order, book);
                    EmitPriceChanged();
                }
            }
        }

        private (decimal?, decimal?) GetBestPrice(Order order, SortedDictionary<decimal, decimal> book)
        {
            if (order.Quantity == 0)
            {
                book.Remove(order.Price);
                return GetBestPriceFromBook(book);
            }

            return (order.Price, order.Quantity);
            
        }
    }
}
