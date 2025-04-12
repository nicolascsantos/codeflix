using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.UnitTests.Common;
using Moq;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.Common
{
    public class GenreUseCasesBaseFixture : BaseFixture
    {
        public DomainEntity.Genre GetValidGenre()
            => new(GetValidGenreName(), GetRandomBoolean());

        public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
        {
            var genre = new DomainEntity.Genre(GetValidGenreName(), isActive ?? GetRandomBoolean());
            categoriesIds?.ForEach(genre.AddCategory);
            return genre;
        }

        public List<DomainEntity.Genre> GetExampleGenresList(int count = 10)
            => Enumerable.Range(1, count).Select(_ =>
                {
                    var genre = new DomainEntity.Genre
                    (
                        GetValidGenreName(),
                        GetRandomBoolean()
                    );
                    GetRandomIdsList().ForEach(genre.AddCategory);
                    return genre;
            }).ToList();
        

        public CreateGenreInput GetExampleInput()
            => new
            (
                GetValidGenreName(),
                GetRandomBoolean()
            );

        public CreateGenreInput GetExampleInput(string? name)
           => new
           (
               name!,
               GetRandomBoolean()
           );

        public List<Guid> GetRandomIdsList(int? count = null)
            => Enumerable.Range(1, count ?? (new Random().Next(1, 10)))
                .Select(_ => Guid.NewGuid())
                .ToList();


        public CreateGenreInput GetExampleInputWithCategories()
        {
            var numberOfCategoriesIds = (new Random()).Next(1, 10);
            var categoriesIds = Enumerable.Range(1, numberOfCategoriesIds)
                .Select(_ => Guid.NewGuid()).ToList();
            return new(GetValidGenreName(), GetRandomBoolean(), categoriesIds);
        }

        public Mock<ICategoryRepository> GetCategoriesRepositoryMock()
            => new();

        public string GetValidGenreName()
            => Faker.Commerce.Categories(1)[0];

        public Mock<IGenreRepository> GetGenreRepositoryMock()
            => new();

        public Mock<IUnitOfWork> GetGenreUnitOfWorkMock()
            => new();
    }
}
