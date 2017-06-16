using System;

namespace MessageSender
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;

    class Program
    {
        private static QueueClient queueClient;
        private const string ServiceBusConnectionString = "Endpoint=sb://tomservicebusdemo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=zmcXIuSOZCUxDANj6XvdrU783YUDsIWRkCm5m6ijYsQ=";
        private const string QueueName = "ProductQueue";

        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
            await SendMessagesToQueue(10);

            // Close the client after the ReceiveMessages method has exited.
            await queueClient.CloseAsync();

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        private static async Task SendMessagesToQueue(int numMessagesToSend)
        {
            for (var i = 0; i < numMessagesToSend; i++)
            {
                try
                {
                    // Create a new brokered message to send to the queue
                    //var message = new Message(Encoding.UTF8.GetBytes($"Message {i}"));

                    // Write the body of the message to the console
                    //Console.WriteLine($"Sending message: {Encoding.UTF8.GetString(message.Body)}");

                    ProductDto productDto =new ProductDto() { Id = "876", Name="QueueMessage"};
                    byte bytes = Convert.tob(productDto);
                    var message = new Message(bytes);

                    // Send the message to the queue
                    await queueClient.SendAsync(message);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                // Delay by 10 milliseconds so that the console can keep up
                await Task.Delay(10);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
        }

        public static byte[] ProtoSerialize<T>(T record) where T : class
        {
            if (null == record) return null;

            try
            {
                using (var stream = new MemoryStream())
                {
                  Protobuf  Serializer.Serialize(stream, record);
                    return stream.ToArray();
                }
            }
            catch
            {
                // Log error
                throw;
            }
        }
    }

    public class ProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}