using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.CreateGenre
{
    [Collection(nameof(CreateGenreAPITestFixture))]
    public class CreateGenreAPITest
    {
        private readonly CreateGenreAPITestFixture _fixture;

        public CreateGenreAPITest(CreateGenreAPITestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateGenre))]
        [Trait("EndToEnd/API", "Genre/Create - Endpoints")]
        public async Task CreateGenre()
        {
            var genreApiInput = new CreateGenreInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandomBoolean()
            );
            var (response, output) = await _fixture.APIClient
                .Post<APIResponse<GenreModelOutput>>("/api/genres/", genreApiInput);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
            output.Should().NotBeNull();
            output.Data.Id.Should().NotBeEmpty();
            output.Data.Name.Should().Be(genreApiInput.Name);
            output.Data.IsActive.Should().Be(genreApiInput.IsActive);
            var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb.Id.Should().NotBeEmpty();
            genreFromDb.Name.Should().Be(genreApiInput.Name);
            genreFromDb.IsActive.Should().Be(genreApiInput.IsActive);
        }

        [Fact(DisplayName = nameof(CreateGenreWithRelations))]
        [Trait("EndToEnd/API", "Genre/Create - Endpoints")]
        public async Task CreateGenreWithRelations()
        {
            var exampleCategories = _fixture.GetExampleCategoriesList();
            await _fixture.CategoryPersistence.InsertList(exampleCategories);
            var relatedCategories = exampleCategories
                .Skip(3)
                .Take(3)
                .Select(x => x.Id)
                .ToList();


            var genreApiInput = new CreateGenreInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandomBoolean(),
                relatedCategories
            );
            var (response, output) = await _fixture.APIClient
                .Post<APIResponse<GenreModelOutput>>("/api/genres/", genreApiInput);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
            output.Should().NotBeNull();
            output.Data.Id.Should().NotBeEmpty();
            output.Data.Name.Should().Be(genreApiInput.Name);
            output.Data.IsActive.Should().Be(genreApiInput.IsActive);
            output.Data.Categories.Should().HaveCount(relatedCategories.Count);
            var outputRelatedCategoriesIds = output.Data.Categories.Select(x => x.Id).ToList();
            outputRelatedCategoriesIds.Should().BeEquivalentTo(relatedCategories);
            var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb.Id.Should().NotBeEmpty();
            genreFromDb.Name.Should().Be(genreApiInput.Name);
            genreFromDb.IsActive.Should().Be(genreApiInput.IsActive);
            var relationsFromDb = await _fixture.Persistence
                .GetGenresCategoriesRelationsByGenreId(output.Data.Id);
            relationsFromDb.Should().NotBeNull();
            relationsFromDb.Should().HaveCount(relatedCategories.Count);
            var relatedCategoriesFromDb = relationsFromDb.Select(x => x.CategoryId).ToList();
            relatedCategoriesFromDb.Should().BeEquivalentTo(relatedCategories);
        }

        [Fact(DisplayName = nameof(ErrorWithInvalidRelations))]
        [Trait("EndToEnd/API", "Genre/Create - Endpoints")]
        public async Task ErrorWithInvalidRelations()
        {
            var exampleCategories = _fixture.GetExampleCategoriesList();
            await _fixture.CategoryPersistence.InsertList(exampleCategories);
            var relatedCategories = exampleCategories
                .Skip(3)
                .Take(3)
                .Select(x => x.Id)
                .ToList();
            var invalidCategoryId = Guid.NewGuid();
            relatedCategories.Add(invalidCategoryId);

            var genreApiInput = new CreateGenreInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandomBoolean(),
                relatedCategories
            );
            var (response, output) = await _fixture.APIClient
                .Post<ProblemDetails>("/api/genres/", genreApiInput);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output.Type.Should().Be("RelatedAggregate");
            output.Detail.Should().Be($"Related category id or ids not found: '{invalidCategoryId}'");
        }
    }
}
