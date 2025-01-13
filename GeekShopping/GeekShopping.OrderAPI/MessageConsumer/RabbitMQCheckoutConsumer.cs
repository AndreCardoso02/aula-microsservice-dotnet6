
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model.Base;
using GeekShopping.OrderAPI.Repository;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using GeekShopping.OrderAPI.RabbitMQSender;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly OrderRepository _repository;
        private IConnection _connection;
        private IChannel _channel;
        private IRabbitMQMessageSender _rabbitMQMessageSender;

        public RabbitMQCheckoutConsumer(OrderRepository repository, IRabbitMQMessageSender rabbitMqMessageSender)
        {
            _repository = repository;
            _rabbitMQMessageSender = rabbitMqMessageSender;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;
            var res = _channel.QueueDeclareAsync(queue: "checkoutqueue", false, false, false, arguments: null).Result;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (chanel, evt) => 
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                CheckoutHeaderVO vo = JsonSerializer.Deserialize<CheckoutHeaderVO>(content)!;
                ProcessOrder(vo).GetAwaiter().GetResult();
                await _channel.BasicAckAsync(evt.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync("checkoutqueue", false, consumer);
            //return Task.CompletedTask;
        }

        private async Task ProcessOrder(CheckoutHeaderVO vo)
        {
            OrderHeader order = new()
            {
                UserId = vo.UserId,
                FirstName = vo.FirstName,
                LastName = vo.LastName,
                OrderDetails = new List<OrderDetail>(),
                CardNumber = vo.CardNumber,
                CouponCode = vo.CouponCode,
                CVV = vo.CVV,
                DiscountAmount = vo.DiscountAmount,
                Email = vo.Email,
                ExpiryMonthYear = vo.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                PaymentStatus = false,
                PhoneNumber = vo.PhoneNumber,
                PurchaseDate = vo.DateTime,
                PurchaseAmount = vo.PurchaseAmount
            };

            foreach (var details in vo.CartDetails)
            {
                OrderDetail detail = new()
                {
                    ProductId = details.ProductId,
                    ProductName = details.Product!.Name!,
                    Price = details.Product.Price,
                    Count = details.Count,
                };
                order.TotalItens += details.Count;
                order.OrderDetails.Add(detail);
            }

            await _repository.AddOrder(order);

            // Publicando as ordens de pagamento
            PaymentVO paymentVo = new()
            {
                Name = order.FirstName + " " + order.LastName,
                CardNumber = order.CardNumber,
                CVV = order.CVV,
                Email = order.Email,
                ExpiryMonthYear = order.ExpiryMonthYear,
                OrderId = order.Id,
                PurchaseAmount = order.PurchaseAmount,
            };

            try
            {
                _rabbitMQMessageSender.SendMessage(paymentVo, "orderpaymentprocessqueue");
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
