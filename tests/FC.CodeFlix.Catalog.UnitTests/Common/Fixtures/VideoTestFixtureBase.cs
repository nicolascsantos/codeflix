using Bogus;
using FC.CodeFlix.Catalog.Domain.Enum;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;


namespace FC.CodeFlix.Catalog.UnitTests.Common.Fixtures
{
    public abstract class VideoTestFixtureBase : BaseFixture
    {
        public DomainEntity.Video GetValidVideo()
            => new(GetValidTitle(), GetValidDescription(), GetValidYearLaunched(), true, true, GetValidDuration(), GetRandomRating());

        public Rating GetRandomRating()
        {
            var enumValues = Enum.GetValues<Rating>();
            var random = new Random();
            return enumValues[random.Next(enumValues.Length)];
        }

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

        public string GetValidImagePath()
            => Faker.Image.PicsumUrl();

        public string GetValidMediaPath()
        {
            var exampleMedias = new string[]
            {
                "www.googlestorage.com/file-example.mp4",
                "www.storage.com/file-example.mp4",
                "www.S3.com.br/file-example.mp4",
                "www.glg.com/video.mp4",
                "www.azure.com/example.mp4",
            };

            var random = new Random();

            return exampleMedias[random.Next(exampleMedias.Length)];
        }

        public DomainEntity.Media GetValidMedia()
            => new(GetValidMediaPath());
    }
}
