using FC.Codeflix.Catalog.IntegrationTests.Base;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common
{
    public class GenreUseCaseBaseFixture : BaseFixture
    {
        public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null, string? name = null)
        {
            var genre = new DomainEntity.Genre(name ?? GetValidGenreName(), isActive ?? GetRandomBoolean());
            categoriesIds?.ForEach(genre.AddCategory);
            return genre;
        }

        public CreateGenreInput GetExampleInput()
           => new
           (
               GetValidGenreName(),
               GetRandomBoolean()
           );

        public string GetValidGenreName()
            => Faker.Commerce.Categories(1)[0];

        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

        public List<DomainEntity.Category> GetExampleCategoriesList(int length = 10) => Enumerable.Range(1, length).Select(
           _ => GetExampleCategory())
                   .ToList();

        public DomainEntity.Category GetExampleCategory()
        {
            return new DomainEntity.Category(
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
            );
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

        public List<DomainEntity.Genre> GetExampleListGenres(int count = 10)
            => Enumerable.Range(1, count).Select(_ => GetExampleGenre()).ToList();

        public List<DomainEntity.Genre> GetExampleListGenresByNames(List<string> names)
            => names.Select(name => GetExampleGenre(name: name)).ToList();
    }
}
