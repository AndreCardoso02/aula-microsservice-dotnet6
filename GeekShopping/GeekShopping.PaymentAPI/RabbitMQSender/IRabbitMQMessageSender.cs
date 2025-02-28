using GeekShopping.MessageBus;

namespace GeekShopping.PaymentAPI.RabbitMQSender
{
    public interface IRabbitMQMessageSender
    {
        Task SendMessage(BaseMessage message); // com exchange nn precisamos da fila
    }
}
