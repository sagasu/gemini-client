﻿using Newtonsoft.Json.Linq;
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
        // search: O(FixedSizeOfArray*FractionalPrecision) 
        // insert, removal: O(1) 
        // access to best price: worst scenario is O(FixedSizeOfArray*FractionalPrecision)
        //  https://github.com/dotnet/runtime/blob/main/src/libraries/System.Collections/src/System/Collections/Generic/SortedDictionary.cs
        private const int FixedSizeOfArray = 60000;
        private const int FractionalPrecision = 100;
        private readonly decimal[,] _bids = new decimal[FixedSizeOfArray, FractionalPrecision];
        private readonly decimal[,] _asks = new decimal[FixedSizeOfArray, FractionalPrecision];

        private decimal bestBidPrice;
        private decimal bestBidQuantity;

        private decimal bestAskPrice;
        private decimal bestAskQuantity;

        public void AddOrder(Order order)
        {
            var book = order.Type == OrderType.Buy ? _bids : _asks;

            UpdateBestPriceAndQuantityForBook(order);

            var (intPart, fractionalPart) = GetDecimalAndFractionPart(order, book);

            book[intPart, fractionalPart] += order.Quantity;
        }
        public void RemoveOrder(Order order)
        {
            var book = order.Type == OrderType.Buy ? _bids : _asks;
            UpdateBestPriceAndQuantityForBook(order);

            var (intPart, fractionalPart)= GetDecimalAndFractionPart(order, book);

            book[intPart, fractionalPart] -= order.Quantity;
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

        private (int, int) GetDecimalAndFractionPart(Order order, decimal[,] book)
        {
            var intPart = (int)order.Price;
            var fractionalPart = (int)((order.Price - intPart) % 1.0m);
            return (intPart,fractionalPart);
        }

        public (decimal bestBidPrice, decimal bestBidQuantity) GetBestBid() => (bestBidPrice, bestBidQuantity);

        public (decimal bestAskPrice, decimal bestAskQuantity) GetBestAsk() => (bestAskPrice, bestAskQuantity);

    }
}
