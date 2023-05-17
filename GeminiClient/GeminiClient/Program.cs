// See https://aka.ms/new-console-template for more information

using CsvHelper;
using CsvHelper.Configuration;
using GeminiClient.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Runtime;
using Websocket.Client;
using static System.Net.Mime.MediaTypeNames;

Console.WriteLine("Hello, World!");

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
            ExportToCSV(msg);

            

            if (msg.ToString().ToLower() == "connected")
            {
                var data = "";
                client.Send(data);
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

void ExportToCSV(ResponseMessage responseMessage)
{
    var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
    var message = JsonConvert.DeserializeObject<GeminiWebSocketMessage>(responseMessage.Text, settings);

    var config = new CsvConfiguration(CultureInfo.InvariantCulture);
    config.HasHeaderRecord = false;

    using (var writer = new StreamWriter("file.csv", true))
    using (var csv = new CsvWriter(writer, config))
    {
        csv.WriteRecords(message?.events);
    }
}