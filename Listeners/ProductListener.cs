namespace DocumentDbDemo.Listeners
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Autofac;
    using Dtos;
    using Microsoft.Azure.ServiceBus;
    using Pocos;
    using Repository;

    public class ProductListener : IStartable, IDisposable
    {
        private readonly IProductRepository _productRepository;
        private static QueueClient _queueClient;
        private const string ServiceBusConnectionString = "Endpoint=sb://tomservicebusdemo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=zmcXIuSOZCUxDANj6XvdrU783YUDsIWRkCm5m6ijYsQ=";
        private const string QueueName = "ProductQueue";

        public ProductListener(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void Start()
        {
            _queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);
            Task.Run(() => RecieveMessages());
        }

        public void RecieveMessages()
        {
            try
            {
                Console.WriteLine("Listening for Products...");
                // Register a OnMessage callback
                _queueClient.RegisterMessageHandler(
                    async (message, token) =>
                    {
                        // Process the message
                        Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

                        var productPoco = FromByteArray<ProductDto>(message.Body);
                        await ProductReceived(productPoco);

                        // Complete the message so that it is not received again.
                        // This can be done only if the queueClient is opened in ReceiveMode.PeekLock mode.
                        await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
                    });
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
            }
        }

        public T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryReader bf = new BinaryReader(ms);
                object obj = bf.Read();
                return (T)obj;
            }
        }

        public async Task ProductReceived(ProductDto productDto)
        {
            var productPoco = MapProductPoco(productDto);

            await _productRepository.Upsert(productPoco);
        }

        private static ProductPoco MapProductPoco(ProductDto productDto)
        {
            ProductPoco productPoco = new ProductPoco()
            {
                Id = productDto.Id,
                Name = productDto.Name
            };
            return productPoco;
        }

        public void Dispose()
        {
            _queueClient.CloseAsync().Wait();
        }
    }
}
