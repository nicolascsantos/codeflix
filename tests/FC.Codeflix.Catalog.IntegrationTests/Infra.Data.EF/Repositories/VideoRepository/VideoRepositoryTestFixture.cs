using FC.Codeflix.Catalog.IntegrationTests.Base;
using FC.CodeFlix.Catalog.Domain.Enum;
using System.Linq;
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

        public DomainEntity.CastMember GetExampleCastMember()
            => new DomainEntity.CastMember(
                GetValidCastMemberName(),
                GetRandomCastMemberType()
            );

        public List<DomainEntity.CastMember> GetRandomCastMembersList()
            => Enumerable.Range(0, Random.Shared.Next(1, 5))
            .Select(_ =>
                new DomainEntity.CastMember(
                    GetValidCastMemberName(),
                    GetRandomCastMemberType()
                )).ToList();

        public string GetValidCastMemberName()
            => Faker.Name.FullName();

        public CastMemberType GetRandomCastMemberType()
            => (CastMemberType)new Random().Next(1, 2);

        public string GetValidCategoryName()
        {
            var categoryName = "";
            while (categoryName.Length < 3)
                categoryName = Faker.Commerce.Categories(1)[0];
            if (categoryName.Length > 255)
                categoryName = categoryName[..255];
            return categoryName;
        }

        public string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            while (categoryDescription.Length > 10_000)
                categoryDescription = categoryDescription[..10_000];
            return categoryDescription;
        }

        public DomainEntity.Category GetValidCategory()
            => new(GetValidCategoryName(),
                GetValidCategoryDescription()
            );

        public List<DomainEntity.Category> GetRandomCategoriesList()
            => Enumerable.Range(0, Random.Shared.Next(1, 5))
            .Select(_ =>
                new DomainEntity.Category(
                    GetValidCategoryName(),
                    GetValidCategoryDescription()
                )).ToList();

        public string GetGenreValidName()
            => Faker.Commerce.Categories(1)[0];

        public DomainEntity.Genre GetExampleGenre(bool isActive = true, List<Guid>? categoriesIdsList = null)
        {
            var genre = new DomainEntity.Genre(GetGenreValidName(), isActive);
            if (categoriesIdsList is not null)
                foreach (var categoryId in categoriesIdsList)
                    genre.AddCategory(categoryId);
            return genre;
        }

        public List<DomainEntity.Genre> GetRandomGenresList()
            => Enumerable.Range(0, Random.Shared.Next(1, 5))
            .Select(_ =>
                new DomainEntity.Genre(
                    GetGenreValidName(),
                    GetRandomBoolean()
                )).ToList();

        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;
    }
}
