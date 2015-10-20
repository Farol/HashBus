using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{

    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        BusConfiguration busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("HashBus.Client");
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.EnableInstallers();
        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.SendFailedMessagesTo("error");

        using (IBus bus = await Bus.Create(busConfiguration).StartAsync())
        {
            await SendOrder(bus);
        }
    }


    #region SendOrder
    static async Task SendOrder(IBus bus)
    {

        Console.WriteLine("Press enter to send a message");
        Console.WriteLine("Press any key to exit");

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }
            Guid id = Guid.NewGuid();

            PlaceOrder placeOrder = new PlaceOrder
            {
                Product = "New shoes",
                Id = id
            };
            await bus.SendAsync("HashBus.Server", placeOrder);

            Console.WriteLine("Sent a new PlaceOrder message with id: {0}", id.ToString("N"));

        }

    }
    #endregion
}
