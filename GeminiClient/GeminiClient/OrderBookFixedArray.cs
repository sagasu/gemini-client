using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient
{
    internal class OrderBookFixedArray
    {
        // Fixed Array size
        // search: O(log n) 
        // insert, removal: O(1) 
        // on average access to lowest price is O(1) but worst scenario is O(log n)
        //  https://github.com/dotnet/runtime/blob/main/src/libraries/System.Collections/src/System/Collections/Generic/SortedDictionary.cs

        private readonly decimal[,] _bids = new decimal[60000,100];
        private readonly decimal[,] _asks = new decimal[60000,100];

        public OrderBookFixedArray()
        {
        }

        private decimal bestBidPrice;
        private decimal bestBidQuantity;

        private decimal bestAskPrice;
        private decimal bestAskQuantity;

        public void AddOrder(Order order)
        {
            decimal[,] book;
            if (order.Type == OrderType.Buy)
            {
                if (order.Price >= bestBidPrice)
                {
                    bestBidPrice = order.Price;
                    bestBidQuantity = order.Quantity;
                }
                    
                book = _bids;
            }
            else
            {
                if (order.Price <= bestAskPrice)
                {
                    bestBidPrice = order.Price;
                    bestBidQuantity = order.Quantity;
                }

                book = _asks;
            }

            var (intPart, fractionalPart) = GetDecimalAndFractionPart(order);

            book[intPart, fractionalPart] += order.Quantity;
        }

        public void RemoveOrder(Order order)
        {
            decimal[,] book;
            if (order.Type == OrderType.Buy)
            {
                if (order.Price >= bestBidPrice)
                {
                    bestBidPrice = order.Price;
                    bestBidQuantity = order.Quantity;
                }

                book = _bids;
            }
            else
            {
                if (order.Price <= bestAskPrice)
                {
                    bestBidPrice = order.Price;
                    bestBidQuantity = order.Quantity;
                }

                book = _asks;
            }

            var (intPart, fractionalPart)= GetDecimalAndFractionPart(order);

            book[intPart, fractionalPart] -= order.Quantity;
        }

        private (int,int) GetDecimalAndFractionPart(Order order)
        {
            var book = order.Type == OrderType.Buy ? _bids : _asks;
            var intPart = (int)order.Price;
            var fractionalPart = (int)((order.Price - intPart) % 1.0m);
            return (intPart,fractionalPart);
        }

        public (decimal bestBidPrice, decimal bestBidQuantity) GetBestBid() => (bestBidPrice, bestBidQuantity);

        public (decimal bestAskPrice, decimal bestAskQuantity) GetBestAsk() => (bestAskPrice, bestAskQuantity);

    }
}
