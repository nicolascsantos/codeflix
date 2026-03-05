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
            => await _context.SaveChangesAsync(cancellationToken);

        public async Task Rollback(CancellationToken cancellationToken)
            => await Task.CompletedTask;
    }
}
