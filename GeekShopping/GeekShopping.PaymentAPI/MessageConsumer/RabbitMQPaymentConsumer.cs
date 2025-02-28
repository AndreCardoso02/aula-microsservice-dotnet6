using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentProcessor;

namespace GeekShopping.PaymentAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private IConnection _connection;
        private IChannel _channel;
        private IRabbitMQMessageSender _rabbitMQMessageSender;
        private readonly IProcessPayment _processPayment;

        public RabbitMQPaymentConsumer(IProcessPayment processPayment, IRabbitMQMessageSender rabbitMqMessageSender)
        {
            _processPayment = processPayment;
            _rabbitMQMessageSender = rabbitMqMessageSender;
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
            var result = _processPayment.PaymentProcessor();

            UpdatePaymentResultMessage paymentResultMessage = new UpdatePaymentResultMessage()
            {
                Status = result,
                OrderId = vo.OrderId,
                Email = vo.Email,
            };
            
            try
            {
                await _rabbitMQMessageSender.SendMessage(paymentResultMessage);
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
