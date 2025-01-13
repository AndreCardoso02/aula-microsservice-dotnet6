using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentProcessor;

namespace GeekShopping.PaymentAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly IProcessPayment _processPayment;

        public RabbitMQPaymentConsumer(IProcessPayment processPayment)
        {
            _processPayment = processPayment;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
            var res = _channel.QueueDeclareAsync(queue: "orderpaymentprocessqueue", false, false, false, arguments: null).Result;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (chanel, evt) => 
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                PaymentMessage vo = JsonSerializer.Deserialize<PaymentMessage>(content)!;
                ProcessPayment(vo).GetAwaiter().GetResult();
                await _channel.BasicAckAsync(evt.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync("orderpaymentprocessqueue", false, consumer);
            //return Task.CompletedTask;
        }

        private async Task ProcessPayment(PaymentMessage vo)
        {
            try
            {
                //_rabbitMQMessageSender.SendMessage(paymentVo, "orderpaymentprocessqueue");
            }
            catch (Exception e)
            {
                // Log
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
