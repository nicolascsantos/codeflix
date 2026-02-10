namespace FC.CodeFlix.Catalog.Domain.SeedWork
{
    public interface IDomainEventPublisher
    {
        Task PublishAsync(DomainEvent domainEvent);
    }
}
