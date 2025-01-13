using GeekShopping.MessageBus;

namespace GeekShopping.OrderAPI.RabbitMQSender
{
    public interface IRabbitMQMessageSender
    {
        Task SendMessage(BaseMessage message, string queueName);
    }
}
