namespace Q10.TaskManager.Application.Interfaces
{
    public interface IMessageQueueRepository
    {
        Task PublishAsync<T>(T message, string queueName) where T : class;
        Task StartConsumingAsync<T>(string queueName, Func<T, Task> onMessage) where T : class;
    }
}

