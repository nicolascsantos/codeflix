using FC.CodeFlix.Catalog.UnitTests.Common;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.Video
{
    [CollectionDefinition(nameof(VideoTestFixture))]
    public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture> { }

    public class VideoTestFixture : BaseFixture
    {
        public DomainEntity.Video GetValidVideo()
            => new(GetValidTitle(), GetValidDescription(), GetValidYearLaunched(), true, true, GetValidDuration());

        public string GetValidTitle()
            => Faker.Lorem.Letter(100);

        public string GetTooLongTitle()
            => Faker.Lorem.Letter(400);

        public string GetTooLongDescription()
            => Faker.Lorem.Letter(4001);


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
    }
}
