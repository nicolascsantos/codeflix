namespace FC.CodeFlix.Catalog.Domain.SeedWork
{
    public interface IDomainEventPublisher
    {
        Task PublishAsync<TDomainEvent>(DomainEvent domainEvent, CancellationToken cancellationToken)
            where TDomainEvent : DomainEvent;
    }
}
