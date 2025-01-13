using GeekShopping.MessageBus;

namespace GeekShopping.PaymentAPI.RabbitMQSender
{
    public interface IRabbitMQMessageSender
    {
        Task SendMessage(BaseMessage message, string queueName);
    }
}
