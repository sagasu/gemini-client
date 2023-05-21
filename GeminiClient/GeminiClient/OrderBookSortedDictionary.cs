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

        private readonly SortedDictionary<decimal, decimal> _bids;
        private readonly SortedDictionary<decimal, decimal> _asks;

        private decimal? _bestBidPrice;
        private decimal? _bestBidQuantity;
        private decimal? _bestAskPrice;
        private decimal? _bestAskQuantity;

        public KeyValuePair<decimal, decimal> GetBestBid() => _bids.FirstOrDefault();
        public KeyValuePair<decimal, decimal> GetBestAsk() => _asks.FirstOrDefault();

        public OrderBookSortedDictionary()
        {
            _bids = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Create((x, y) => y.CompareTo(x)));
            _asks = new SortedDictionary<decimal, decimal>();
        }

        public void AddOrder(Order order)
        {
            var book = order.Type == OrderType.Buy ? _bids : _asks;

            book[order.Price] = order.Quantity;
            CacheBestPrice(order, book);
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
                    CacheBestPrice(order);
                }
            }
        }

        private void CacheBestPrice(Order order)
        {
            if (order.Type == OrderType.Buy)
            {
                if (order.Price == _bestBidPrice)
                    CacheBestBid();
            }
            else
            {
                if (order.Price == _bestAskPrice)
                    CacheBestAsk();
            }
        }

        private void CacheBestBid()
        {
            if (_bids.Count == 0)
            {
                _bestBidPrice = null;
                _bestBidQuantity = null;
            }
            else
            {
                Console.WriteLine("hitting best bid");
                var bestBid = GetBestBid();
                _bestBidPrice = bestBid.Key;
                _bestBidQuantity = bestBid.Value;
            }

            PrintState();
        }

        private void CacheBestAsk()
        {
            if (_asks.Count == 0)
            {
                _bestAskPrice = null;
                _bestAskQuantity = null;
            }
            else
            {
                Console.WriteLine("hitting best ask");
                var bestBid = GetBestAsk();
                _bestAskPrice = bestBid.Key;
                _bestAskQuantity = bestBid.Value;
            }

            PrintState();
        }

        private void PrintState() => Console.WriteLine($"bp:{_bestBidPrice} bq:{_bestBidQuantity} ap:{_bestAskPrice} aq:{_bestAskQuantity} asks:{_asks.Count} bids:{_bids.Count}");
        
        void CacheBestPrice(Order order, SortedDictionary<decimal, decimal> book)
        {
            if (order.Type == OrderType.Buy)
            {
                if (order.Price >= _bestBidPrice || _bestBidPrice is null)
                {
                    if (order.Quantity == 0)
                    {
                        book.Remove(order.Price);
                        CacheBestBid();
                    }
                    else
                    {
                        _bestBidPrice = order.Price;
                        _bestBidQuantity = order.Quantity;
                        PrintState();
                    }
                }
            }
            else
            {
                if (order.Price <= _bestAskPrice || _bestAskPrice is null)
                {
                    if (order.Quantity == 0)
                    {
                        book.Remove(order.Price);
                        CacheBestAsk();
                    }
                    else
                    {
                        _bestAskPrice = order.Price;
                        _bestAskQuantity = order.Quantity;
                        PrintState();
                    }
                }
            }
        }
    }
}
