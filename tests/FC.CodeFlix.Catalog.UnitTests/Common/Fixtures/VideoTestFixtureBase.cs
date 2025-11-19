using Bogus;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Enum;
using System.Text;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;


namespace FC.CodeFlix.Catalog.UnitTests.Common.Fixtures
{
    public abstract class VideoTestFixtureBase : BaseFixture
    {
        public DomainEntity.Video GetValidVideo()
            => new(GetValidTitle(), GetValidDescription(), GetValidYearLaunched(), true, true, GetValidDuration(), GetRandomRating());

        public DomainEntity.Video GetValidVideoWithAllProperties()
        {
            var video = new DomainEntity.Video(
                GetValidTitle(),
                GetValidDescription(),
                GetValidYearLaunched(),
                GetRandomBoolean(),
                GetRandomBoolean(),
                GetValidDuration(),
                GetRandomRating()
            );
            video.UpdateThumb(GetValidImagePath());
            video.UpdateThumbHalf(GetValidImagePath());
            video.UpdateBanner(GetValidImagePath());
            video.UpdateMedia(GetValidImagePath());
            video.UpdateTrailer(GetValidImagePath());

            var random = new Random();
            Enumerable.Range(1, random.Next(2, 5))
                .ToList().ForEach(_ => video.AddCastMember(Guid.NewGuid()));

            Enumerable.Range(1, random.Next(2, 5))
                .ToList().ForEach(_ => video.AddCategory(Guid.NewGuid()));

            Enumerable.Range(1, random.Next(2, 5))
                .ToList().ForEach(_ => video.AddGenre(Guid.NewGuid()));

            return video;
        }

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

        public FileInput GetValidImageFileInput()
        {
            var streamExample = new MemoryStream(Encoding.ASCII.GetBytes("test"));
            var thumbFileInput = new FileInput("jpg", streamExample);
            return thumbFileInput;
        }

        public FileInput GetValidMediaFileInput()
        {
            var streamExample = new MemoryStream(Encoding.ASCII.GetBytes("test"));
            var thumbFileInput = new FileInput("mp4", streamExample);
            return thumbFileInput;
        }
    }
}
