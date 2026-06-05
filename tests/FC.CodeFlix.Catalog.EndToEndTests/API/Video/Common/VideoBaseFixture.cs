using FC.CodeFlix.Catalog.API.APIModels.Video;
using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.Common;
using FC.CodeFlix.Catalog.EndToEndTests.API.Genre.Common;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Video.Common
{
    [CollectionDefinition(nameof(VideoBaseFixture))]
    public class VideoBaseFixtureCollection : ICollectionFixture<VideoBaseFixture> { }

    public class VideoBaseFixture : GenreBaseFixture
    {
        public VideoPersistence VideoPersistence { get; set; }
        public readonly CastMemberPersistence CastMemberPersistence;

        public VideoBaseFixture() : base()
        {
            VideoPersistence = new VideoPersistence(_dbContext);
            CastMemberPersistence = new CastMemberPersistence(_dbContext);
        }

        #region Video

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

        #endregion

        #region CastMembers

        public List<DomainEntity.CastMember> GetExampleCastMembersList(int length = 10)
            => Enumerable.Range(1, length)
                .Select(_ =>
                {
                    Thread.Sleep(1);
                    return GetExampleCastMember();
                })
                .ToList();

        public DomainEntity.CastMember GetExampleCastMember()
          => new DomainEntity.CastMember(
              GetValidName(),
              GetRandomCastMemberType()
          );

        public CastMemberType GetRandomCastMemberType()
            => (CastMemberType)(new Random().Next(1, 2));

        public string GetValidName()
            => Faker.Name.FullName();

        #endregion

    }
}
