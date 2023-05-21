using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeminiClient
{
    internal class BestPriceUpdater
    {
        internal static decimal[,] UpdateBestPriceAndQuantityForBook(Order order, decimal[,] bids, decimal[,] asks, ref decimal bestBidPrice, ref decimal bestBidQuantity, ref decimal bestAskPrice, ref decimal bestAskQuantity)
        {
            decimal[,] book;
            if (order.Type == OrderType.Buy)
            {
                if (order.Price >= bestBidPrice)
                {
                    bestBidPrice = order.Price;
                    bestBidQuantity = order.Quantity;
                }

                book = bids;
            }
            else
            {
                if (order.Price <= bestAskPrice)
                {
                    bestAskPrice = order.Price;
                    bestAskQuantity = order.Quantity;
                }

                book = asks;
            }

            return book;
        }
    }
}
