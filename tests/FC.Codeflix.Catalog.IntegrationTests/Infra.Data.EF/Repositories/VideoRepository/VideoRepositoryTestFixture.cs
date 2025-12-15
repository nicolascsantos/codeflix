using FC.Codeflix.Catalog.IntegrationTests.Base;
using FC.CodeFlix.Catalog.Domain.Enum;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository
{
    [CollectionDefinition(nameof(VideoRepositoryTestFixture))]
    public class VideoRepositoryTestFixtureCollection : ICollectionFixture<VideoRepositoryTestFixture>
    {
    }

    public class VideoRepositoryTestFixture : BaseFixture
    {
        public DomainEntity.Video GetExampleVideo()
            => new(
                GetValidTitle(),
                GetValidDescription(),
                GetValidYearLaunched(),
                true,
                true, 
                GetValidDuration(),
                GetRandomRating()
            );

        public string GetValidTitle()
            => Faker.Lorem.Letter(100);

        public string GetValidDescription()
            => Faker.Commerce.ProductDescription();

        public int GetValidYearLaunched()
            => Faker.Date.BetweenDateOnly
            (
                new DateOnly(1960, 1, 1),
                new DateOnly(2025, 12, 31)
            ).Year;

        public int GetValidDuration()
           => (new Random()).Next(100, 300);

        public Rating GetRandomRating()
        {
            var enumValues = Enum.GetValues<Rating>();
            var random = new Random();
            return enumValues[random.Next(enumValues.Length)];
        }
    }
}
