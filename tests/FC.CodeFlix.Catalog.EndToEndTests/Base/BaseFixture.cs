using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base
{
    public class BaseFixture
    {
        private readonly string _databaseConnectionString;

        public Faker Faker { get; set; }

        public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }

        public HttpClient HttpClient { get; set; }

        public APIClient APIClient { get; set; }

        public BaseFixture()
        {
            Faker = new Faker();
            WebAppFactory = new CustomWebApplicationFactory<Program>();
            HttpClient = WebAppFactory.CreateClient();
            APIClient = new APIClient(HttpClient);
            var configuration = WebAppFactory.Services.GetService(typeof(IConfiguration));
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            _databaseConnectionString = ((IConfiguration)configuration).GetConnectionString("CatalogDb")!;
        }

        public CodeflixCatalogDbContext CreateDbContext()
        {
            var context = new CodeflixCatalogDbContext(new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseMySql(_databaseConnectionString, ServerVersion.AutoDetect(_databaseConnectionString)).Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        public void CleanPersistence()
        {
            var context = CreateDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
