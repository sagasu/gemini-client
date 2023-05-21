// See https://aka.ms/new-console-template for more information

using CsvHelper;
using CsvHelper.Configuration;
using GeminiClient;
using GeminiClient.Models;
using Newtonsoft.Json;
using System.Globalization;
using Websocket.Client;

Console.WriteLine("Hello, World!");


var orderBook = new OrderBookNaive();
Initialize();

 void Initialize()
{
    Console.CursorVisible = false;
    try
    {
        var exitEvent = new ManualResetEvent(false);
        var url = new Uri("wss://api.gemini.com/v1/marketdata/BTCUSD");

        using var client = new WebsocketClient(url);

        decimal? bestBidValue = null, bestBidQuantity = null, bestAskValue = null, bestAskQuantity = null;

        client.ReconnectTimeout = TimeSpan.FromSeconds(30);
        client.ReconnectionHappened.Subscribe(info =>
        {
            Console.WriteLine("Reconnection happened, type: " + info.Type);
        });
        client.MessageReceived.Subscribe(msg =>
        {
            //Console.WriteLine("Message received: " + msg);
            var webSocketMessage = GetMessage(msg);
            ExportToCSV(webSocketMessage);

            foreach (var geminiWebSocketEvent in webSocketMessage.events)
            {
                var orderType = geminiWebSocketEvent.side == "bid" ? OrderType.Buy : OrderType.Sell;

                if (geminiWebSocketEvent.type == "change")
                {
                    
                    var order = new Order()
                    {
                        Price = geminiWebSocketEvent.price,
                        Quantity = geminiWebSocketEvent.remaining,
                        Type = orderType
                    };

                    if (geminiWebSocketEvent.reason is "init" or "place")
                        orderBook.AddOrder(order);
                    else if (geminiWebSocketEvent.reason is "cancel")
                        orderBook.RemoveOrder(order);
                    
                }
                else if(geminiWebSocketEvent.type == "trade")
                {
                    orderBook.AddOrder(
                        new Order()
                        {
                            Price = geminiWebSocketEvent.price,
                            Quantity = geminiWebSocketEvent.amount,
                            Type = orderType
                        });
                }
            }

        });

        client.Start();
        exitEvent.WaitOne();

    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR: " + ex.ToString());
    }
    Console.ReadKey();
}

GeminiWebSocketMessage? GetMessage(ResponseMessage responseMessage) => JsonConvert.DeserializeObject<GeminiWebSocketMessage>(responseMessage.Text, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

void ExportToCSV(GeminiWebSocketMessage? responseMessage)
{
    var config = new CsvConfiguration(CultureInfo.InvariantCulture);
    config.HasHeaderRecord = false;

    using (var writer = new StreamWriter("file.csv", true))
    using (var csv = new CsvWriter(writer, config))
    {
        csv.WriteRecords(responseMessage?.events);
    }
}