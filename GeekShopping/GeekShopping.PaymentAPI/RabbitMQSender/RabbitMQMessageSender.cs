using GeekShopping.MessageBus;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using GeekShopping.PaymentAPI.Messages;

namespace GeekShopping.PaymentAPI.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _username;
        private IConnection? _connection;

        public RabbitMQMessageSender()
        {
            _hostname="localhost";
            _password="guest";
            _username="guest";
        }


        public async Task SendMessage(BaseMessage message, string queueName)
        {
            if (await ConnectionExists())
            {
                using var _channel = await _connection.CreateChannelAsync();
                await _channel.QueueDeclareAsync(queue: queueName, false, false, false, arguments: null);

                byte[] body = GetMessageAsByteArray(message);

                await _channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body);
            }
        }

        private byte[] GetMessageAsByteArray(BaseMessage message)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var json = JsonSerializer.Serialize<UpdatePaymentResultMessage>((UpdatePaymentResultMessage)message, options);
            var body = Encoding.UTF8.GetBytes(json);
            return body;
        }

        private async Task CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password,
                };

                _connection = await factory.CreateConnectionAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private async Task<bool> ConnectionExists()
        {
            if (_connection != null) return true;
            await CreateConnection();
            return _connection != null;
        }
    }
}
