using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.UnitTests.Common.Fixtures;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.ListVideos
{
    [CollectionDefinition(nameof(ListVideosTestFixture))]
    public class ListVideosTestFixtureCollection : ICollectionFixture<ListVideosTestFixture> { }

    public class ListVideosTestFixture : VideoTestFixtureBase
    {
        public List<DomainEntity.Video> GetVideosExamplesList(int length = 10)
            => Enumerable.Range(1, Random.Shared.Next(2, length))
            .Select(_ => GetValidVideoWithAllProperties())
            .ToList();

        internal (
            List<DomainEntity.Video> Videos,
            List<DomainEntity.Category> Categories,
            List<DomainEntity.Genre> Genres,
            List<DomainEntity.CastMember> CastMembers
        ) GetVideosExamplesListWithRelations()
        {
            var itemsToBeCreated = Random.Shared.Next(2, 10);
            List<DomainEntity.Category> categories = new();
            List<DomainEntity.Genre> genres = new();
            List<DomainEntity.CastMember> castMembers = new();
            var videos = Enumerable.Range(1, itemsToBeCreated)
            .Select(_ => GetValidVideoWithAllProperties())
            .ToList();

            videos.ForEach(video =>
            {
                video.RemoveAllCategories();
                var categoriesAmount = Random.Shared.Next(2, 5);
                for (int i = 0; i < categoriesAmount; i++)
                {
                    var category = GetExampleCategory();
                    categories.Add(category);
                    video.AddCategory(category.Id);
                }

                video.RemoveAllGenres();
                var genresAmount = Random.Shared.Next(2, 5);
                for (int i = 0; i < genresAmount; i++)
                {
                    var genre = GetExampleGenre();
                    genres.Add(genre);
                    video.AddGenre(genre.Id);
                }

                video.RemoveAllCastMembers();
                var castMembersAmount = Random.Shared.Next(2, 5);
                for (int i = 0; i < castMembersAmount; i++)
                {
                    var castMember = GetExampleCastMember();
                    castMembers.Add(castMember);
                    video.AddCastMember(castMember.Id);
                }
            });

            return (videos, categories, genres, castMembers);
        }

        public List<DomainEntity.Video> GetVideosExamplesListWithoutRelations()
        => Enumerable.Range(1, Random.Shared.Next(2, 10))
            .Select(_ => GetValidVideo())
            .ToList();

        private string GetValidCategoryName()
        {
            var categoryName = "";
            while (categoryName.Length < 3)
                categoryName = Faker.Commerce.Categories(1)[0];
            if (categoryName.Length > 255)
                categoryName = categoryName[..255];
            return categoryName;
        }

        private string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            while (categoryDescription.Length > 10_000)
                categoryDescription = categoryDescription[..10_000];
            return categoryDescription;
        }


        private DomainEntity.Category GetExampleCategory()
            => new(
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
            );

        private DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
        {
            var genre = new DomainEntity.Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());
            categoriesIds?.ForEach(genre.AddCategory);
            return genre;
        }

        private string GetValidGenreName()
            => Faker.Commerce.Categories(1)[0];

        private DomainEntity.CastMember GetExampleCastMember()
          => new DomainEntity.CastMember(
              GetValidName(),
              GetRandomCastMemberType()
          );

        private string GetValidName()
            => Faker.Name.FullName();

        private CastMemberType GetRandomCastMemberType()
            => (CastMemberType)(new Random().Next(1, 2));
    }
}
