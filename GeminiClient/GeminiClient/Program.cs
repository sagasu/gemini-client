// See https://aka.ms/new-console-template for more information

using Websocket.Client;

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