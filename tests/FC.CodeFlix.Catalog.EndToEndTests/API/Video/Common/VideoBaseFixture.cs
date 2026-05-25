using FC.CodeFlix.Catalog.API.APIModels.Video;
using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.EndToEndTests.API.Genre.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Video.Common
{
    [CollectionDefinition(nameof(VideoBaseFixture))]
    public class VideoBaseFixtureCollection : ICollectionFixture<VideoBaseFixture> { }

    public class VideoBaseFixture : GenreBaseFixture
    {
        public VideoPersistence VideoPersistence { get; set; }

        public VideoBaseFixture() : base()
        {
            VideoPersistence = new VideoPersistence(_dbContext);
        }

        public CreateVideoAPIInput GetBasicCreateVideoInput()
            => new CreateVideoAPIInput(
                GetValidTitle(),
                GetValidDescription(),
                GetValidYearLaunched(),
                GetRandomBoolean(),
                GetRandomBoolean(),
                GetValidDuration(),
                GetRandomRating(),
                null,
                null,
                null
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

        public Rating GetRandomRating()
        {
            var enumValues = Enum.GetValues<Rating>();
            var random = new Random();
            return enumValues[random.Next(enumValues.Length)];
        }

        public int GetValidDuration()
            => (new Random()).Next(100, 300);
    }
}
