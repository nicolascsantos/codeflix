using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.Infra.Data.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CodeflixCatalogDbContext _context;
        private readonly IDomainEventPublisher _publisher;
        private readonly ILogger<IUnitOfWork> _logger;

        public UnitOfWork(
            CodeflixCatalogDbContext context,
            IDomainEventPublisher publisher,
            ILogger<IUnitOfWork> logger
        )
        {
            _context = context;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task Commit(CancellationToken cancellationToken)
        {
            var aggregateRoots = _context.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(entry => entry.Entity.Events.Any())
                .Select(entry => entry.Entity);
            _logger.LogInformation($"Commit: {aggregateRoots.Count()} agreggate roots with events.");

            var events = aggregateRoots.SelectMany(aggregate => aggregate.Events);

            _logger.LogInformation($"Commit: {events.Count()} events raised.");

            foreach (var @event in events)
            {
                await _publisher.PublishAsync<DomainEvent>(@event, cancellationToken);
            }
            

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Rollback(CancellationToken cancellationToken)
            => await Task.CompletedTask;
    }
}
