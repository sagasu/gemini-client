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
        
        client.ReconnectTimeout = TimeSpan.FromSeconds(30);
        client.ReconnectionHappened.Subscribe(info =>
        {
            Console.WriteLine("Reconnection happened, type: " + info.Type);
        });
        client.MessageReceived.Subscribe(msg =>
        {
            Console.WriteLine("Message received: " + msg);
            var webSocketMessage = GetMessage(msg);
            ExportToCSV(webSocketMessage);

            var geminiWebSocketEvent = webSocketMessage.events[0];
            orderBook.AddOrder(new Order
            {
                Price = geminiWebSocketEvent.price,
                Quantity = geminiWebSocketEvent.remaining,
                Type = geminiWebSocketEvent.side == "bid" ? OrderType.Buy : OrderType.Sell
            });
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