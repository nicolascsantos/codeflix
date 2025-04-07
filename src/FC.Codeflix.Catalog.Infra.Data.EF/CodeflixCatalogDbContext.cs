using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using FC.CodeFlix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF
{
    public class CodeflixCatalogDbContext : DbContext
    {
        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Genre> Genres => Set<Genre>();

        public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        }
    }
}
