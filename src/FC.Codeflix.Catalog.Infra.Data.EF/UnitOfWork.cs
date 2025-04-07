﻿using FC.CodeFlix.Catalog.Application.Interfaces;

namespace FC.Codeflix.Catalog.Infra.Data.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CodeflixCatalogDbContext _context;

        public UnitOfWork(CodeflixCatalogDbContext context)
            => _context = context;

        public async Task Commit(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);

        public async Task Rollback(CancellationToken cancellationToken)
            => await Task.CompletedTask;
    }
}
