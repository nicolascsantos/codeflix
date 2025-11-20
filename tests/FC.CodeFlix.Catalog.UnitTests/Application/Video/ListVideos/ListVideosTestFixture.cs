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
            List<DomainEntity.Genre>
        ) GetVideosExamplesListWithRelations()
        {
            var itemsToBeCreated = Random.Shared.Next(2, 10);
            List<DomainEntity.Category> categories = new();
            List<DomainEntity.Genre> genres = new();
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
            });

            return (videos, categories, genres);
        }

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


        public DomainEntity.Category GetExampleCategory()
            => new(
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
            );

        public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
        {
            var genre = new DomainEntity.Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());
            categoriesIds?.ForEach(genre.AddCategory);
            return genre;
        }

        private string GetValidGenreName()
            => Faker.Commerce.Categories(1)[0];
    }
}
