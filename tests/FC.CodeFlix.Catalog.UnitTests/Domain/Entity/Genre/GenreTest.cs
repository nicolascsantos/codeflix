using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FluentAssertions;
using FC.CodeFlix.Catalog.Domain.Exceptions;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Entity.Genre
{
    [Collection(nameof(GenreTestFixture))]
    public class GenreTest
    {
        private readonly GenreTestFixture _fixture;

        public GenreTest(GenreTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "Genre - Aggregates")]
        public void Instantiate()
        {
            var genreName = _fixture.GetValidName();
            var dateTimeBefore = DateTime.Now;
            var genre = new DomainEntity.Genre(genreName);
            var dateTimeAfter = DateTime.Now.AddSeconds(1);

            genre.Should().NotBeNull();
            genre.Id.Should().NotBeEmpty();
            genre.Name.Should().Be(genreName);
            genre.IsActive.Should().BeTrue();
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
            (genre.CreatedAt >= dateTimeBefore).Should().BeTrue();
            (genre.CreatedAt <= dateTimeAfter).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(InstantiateWithIsActive))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void InstantiateWithIsActive(bool isActive)
        {
            var genreName = _fixture.GetValidName();
            var dateTimeBefore = DateTime.Now;
            var genre = new DomainEntity.Genre(genreName, isActive);
            var dateTimeAfter = DateTime.Now.AddSeconds(1);

            genre.Should().NotBeNull();
            genre.Id.Should().NotBeEmpty();
            genre.Name.Should().Be(genreName);
            genre.IsActive.Should().Be(isActive);
            genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
            (genre.CreatedAt >= dateTimeBefore).Should().BeTrue();
            (genre.CreatedAt <= dateTimeAfter).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(Activate))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void Activate(bool isActive)
        {
            var genre = _fixture.GetExampleGenre(isActive);
            var oldName = genre.Name;
            genre.Activate();
            genre.Should().NotBeNull();
            genre.Id.Should().NotBeEmpty();
            genre.Name.Should().Be(oldName);
            genre.IsActive.Should().BeTrue();
        }

        [Theory(DisplayName = nameof(Deactivate))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void Deactivate(bool isActive)
        {
            var genre = _fixture.GetExampleGenre(isActive);
            var oldName = genre.Name;
            genre.Deactivate();
            genre.Should().NotBeNull();
            genre.Id.Should().NotBeEmpty();
            genre.Name.Should().Be(oldName);
            genre.IsActive.Should().BeFalse();
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Genre - Aggregates")]
        public void Update()
        {
            var validGenre = _fixture.GetExampleGenre();
            var genreWithNewValues = _fixture.GetExampleGenre();
            validGenre.Update(genreWithNewValues.Name);
            validGenre.Name.Should().Be(genreWithNewValues.Name);
            validGenre.IsActive.Should().Be(genreWithNewValues.IsActive);
        }

        [Theory(DisplayName = nameof(InstantiateThrowWhenNameIsEmpty))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void InstantiateThrowWhenNameIsEmpty(string? name)
        {
            Action action = () => new DomainEntity.Genre(name!);
            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Name should not be empty or null.");
        }

        [Theory(DisplayName = nameof(UpdateThrowWhenNameIsEmpty))]
        [Trait("Domain", "Genre - Aggregates")]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void UpdateThrowWhenNameIsEmpty(string? name)
        {
            var genre = _fixture.GetExampleGenre();
            Action action = () => genre.Update(name!);
            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Name should not be empty or null.");
        }

        [Fact(DisplayName = nameof(AddCategory))]
        [Trait("Domain", "Genre - Aggregates")]
        public void AddCategory()
        {
            var genre = _fixture.GetExampleGenre();
            var categoryGuid = Guid.NewGuid();
            genre.AddCategory(categoryGuid);
            genre.Categories.Should().HaveCount(1);
            genre.Categories.Should().Contain(categoryGuid);
        }

        [Fact(DisplayName = nameof(AddTwoCategories))]
        [Trait("Domain", "Genre - Aggregates")]
        public void AddTwoCategories()
        {
            var genre = _fixture.GetExampleGenre();
            var categoryGuid1 = Guid.NewGuid();
            var categoryGuid2 = Guid.NewGuid();
            genre.AddCategory(categoryGuid1);
            genre.AddCategory(categoryGuid2);
            genre.Categories.Should().HaveCount(2);
            genre.Categories.Should().Contain(categoryGuid1);
            genre.Categories.Should().Contain(categoryGuid2);
        }

        [Fact(DisplayName = nameof(RemoveCategory))]
        [Trait("Domain", "Genre - Aggregates")]
        public void RemoveCategory()
        {
            var exampleGuid = Guid.NewGuid();
            var genre = _fixture.GetExampleGenre(
                categoriesIdsList: new List<Guid>() 
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    exampleGuid,
                    Guid.NewGuid(),
                    Guid.NewGuid()
                });

            genre.RemoveCategory(exampleGuid);

            genre.Categories.Should().HaveCount(4);
            genre.Categories.Should().NotContain(exampleGuid);
        }

        [Fact(DisplayName = nameof(RemoveAllCategories))]
        [Trait("Domain", "Genre - Aggregates")]
        public void RemoveAllCategories()
        {
            var exampleGuid = Guid.NewGuid();
            var genre = _fixture.GetExampleGenre(
                categoriesIdsList: new List<Guid>()
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    exampleGuid,
                    Guid.NewGuid(),
                    Guid.NewGuid()
                });

            genre.RemoveAllCategories();

            genre.Categories.Should().HaveCount(0);
        }
    }
}
