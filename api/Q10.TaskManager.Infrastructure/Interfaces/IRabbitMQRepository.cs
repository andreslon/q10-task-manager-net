namespace Q10.TaskManager.Infrastructure.Interfaces
{
    public interface IRabbitMQRepository
    {
        Task PublishAsync<T>(T message, string queueName) where T : class;
        Task StartConsumingAsync<T>(string queueName, Func<T, Task> handler) where T : class;
    }
}
