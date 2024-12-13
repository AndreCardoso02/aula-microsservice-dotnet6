namespace GeekShopping.MessageBus
{
    public interface IMessageBuss
    {
        Task PublicMessage(BaseMessage massage, string queueName);
    }
}
