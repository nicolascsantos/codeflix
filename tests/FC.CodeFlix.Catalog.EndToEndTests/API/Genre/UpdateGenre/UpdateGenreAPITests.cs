using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.CodeFlix.Catalog.API.APIModels.Genre;
using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.UpdateGenre
{
    [Collection(nameof(UpdateGenreAPITestFixture))]
    public class UpdateGenreAPITests
    {
        private readonly UpdateGenreAPITestFixture _fixture;

        public UpdateGenreAPITests(UpdateGenreAPITestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(UpdateGenre))]
        [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
        public async Task UpdateGenre()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var targetGenre = exampleGenres[5];
            await _fixture.Persistence.InsertList(exampleGenres);

            var input = new UpdateGenreAPIInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandomBoolean()
            );

            var (response, output) = await _fixture.APIClient
                .Put<APIResponse<GenreModelOutput>>($"/api/genres/{targetGenre.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Data.Id.Should().Be(targetGenre.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.IsActive.Should().Be((bool)input.IsActive!);
            var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb.Name.Should().Be(input.Name);
            genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        }

        [Fact(DisplayName = nameof(ProblemDetailsWhenNotFound))]
        [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
        public async Task ProblemDetailsWhenNotFound()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var randomGuid = Guid.NewGuid();
            await _fixture.Persistence.InsertList(exampleGenres);

            var input = new UpdateGenreAPIInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandomBoolean()
            );

            var (response, output) = await _fixture.APIClient
                .Put<ProblemDetails>($"/api/genres/{randomGuid}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output.Title.Should().Be("One or more validation errors occurred.");
            output.Detail.Should().Be($"Genre '{randomGuid}' not found.");
            output.Status.Should().Be(StatusCodes.Status404NotFound);

        }

        [Fact(DisplayName = nameof(UpdateGenreWithRelations))]
        [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
        public async Task UpdateGenreWithRelations()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var targetGenre = exampleGenres[5];
            var exampleCategories = _fixture.GetExampleCategoriesList(10);
            var random = new Random();
            var genresCategories = new List<GenresCategories>();

            await _fixture
                .GenresListWithCategoriesRelations(exampleGenres, exampleCategories, genresCategories, _fixture);



            var newRelationsCount = random.Next(2, exampleCategories.Count - 1);
            var newRelatedCategoriesIds = new List<Guid>();

            for (int i = 0; i < newRelationsCount; i++)
            {
                var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                var selectedCategory = exampleCategories[selectedCategoryIndex];
                if (!newRelatedCategoriesIds.Contains(selectedCategory.Id))
                    newRelatedCategoriesIds.Add(selectedCategory.Id);
            }

            var input = new UpdateGenreAPIInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandomBoolean(),
                newRelatedCategoriesIds
            );

            var (response, output) = await _fixture.APIClient
                .Put<APIResponse<GenreModelOutput>>($"/api/genres/{targetGenre.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Data.Id.Should().Be(targetGenre.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.IsActive.Should().Be((bool)input.IsActive!);
            List<Guid> relatedCategoriesIdsFromOutput =
                output.Data.Categories.Select(relation => relation.Id).ToList();
            relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(newRelatedCategoriesIds);
            var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb.Name.Should().Be(input.Name);
            genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
            var genresCategoriesFromDb = await _fixture.Persistence
                .GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
            var relatedCategoriesIdsFromDb = genresCategoriesFromDb.Select(x => x.CategoryId)
                .ToList();
            relatedCategoriesIdsFromDb.Should().BeEquivalentTo(newRelatedCategoriesIds);
        }

        [Fact(DisplayName = nameof(ErrorWhenInvalidRelation))]
        [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
        public async Task ErrorWhenInvalidRelation()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var targetGenre = exampleGenres[5];
            var randomGuid = Guid.NewGuid();
            await _fixture.Persistence.InsertList(exampleGenres);

            var input = new UpdateGenreAPIInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandomBoolean(),
                new List<Guid>() { randomGuid }
            );

            var (response, output) = await _fixture.APIClient
                .Put<ProblemDetails>($"/api/genres/{targetGenre.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output.Type.Should().Be("RelatedAggregate");
            output.Detail.Should().Be($"Related category id or ids not found: '{randomGuid}'");
        }

        [Fact(DisplayName = nameof(PersistsRelationsWhenNotPresentInInput))]
        [Trait("EndToEnd/API", "Genre/Update - Endpoints")]
        public async Task PersistsRelationsWhenNotPresentInInput()
        {
            var exampleGenres = _fixture.GetExampleListGenres();
            var targetGenre = exampleGenres[5];
            var exampleCategories = _fixture.GetExampleCategoriesList(10);
            var random = new Random();
            var genresCategories = new List<GenresCategories>();

            await _fixture
                .GenresListWithCategoriesRelations(exampleGenres, exampleCategories, genresCategories, _fixture);

            var input = new UpdateGenreAPIInput(
                _fixture.GetValidGenreName(),
                _fixture.GetRandomBoolean()
            );

            var (response, output) = await _fixture.APIClient
                .Put<APIResponse<GenreModelOutput>>($"/api/genres/{targetGenre.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Data.Id.Should().Be(targetGenre.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.IsActive.Should().Be((bool)input.IsActive!);
            List<Guid> relatedCategoriesIdsFromOutput =
                output.Data.Categories.Select(relation => relation.Id).ToList();
            relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(targetGenre.Categories);
            var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
            genreFromDb.Should().NotBeNull();
            genreFromDb.Name.Should().Be(input.Name);
            genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
            var genresCategoriesFromDb = await _fixture.Persistence
                .GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
            var relatedCategoriesIdsFromDb = genresCategoriesFromDb.Select(x => x.CategoryId)
                .ToList();
            relatedCategoriesIdsFromDb.Should().BeEquivalentTo(targetGenre.Categories);
        }
    }
}
