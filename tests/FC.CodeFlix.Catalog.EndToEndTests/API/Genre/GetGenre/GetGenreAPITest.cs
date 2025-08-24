using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.GetGenre
{
    [Collection(nameof(GetGenreAPITestFixture))]
    public class GetGenreAPITest
    {
        private readonly GetGenreAPITestFixture _fixture;

        public GetGenreAPITest(GetGenreAPITestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(GetGenre))]
        [Trait("EndToEnd/API", "Genre/GetGenreById - Endpoints")]
        public async Task GetGenre()
        {
            // cadastrar uma lista de genres na persistencia
            var dbContext = _fixture.CreateDbContext();
            var exampleGenresList = _fixture.GetExampleListGenres();
            var targetGenre = exampleGenresList[5];
            await _fixture.Persistence.InsertList(exampleGenresList);

            // busca por um genre especifico por rest
            var (response, output) = await _fixture.APIClient
                .Get<APIResponse<GenreModelOutput>>($"/api/genres/{targetGenre.Id}");

            // testa e ve se o item é o que a gente espera\
            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output!.Data.Should().NotBeNull();
            output.Data.Id.Should().Be(targetGenre.Id);
            output.Data.Name.Should().Be(targetGenre.Name);
            output.Data.IsActive.Should().Be(targetGenre.IsActive);
        }

        [Fact(DisplayName = nameof(ThrowWhenGenreNotFound))]
        [Trait("EndToEnd/API", "Genre/GetGenreById - Endpoints")]
        public async Task ThrowWhenGenreNotFound()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenresList = _fixture.GetExampleListGenres();
            var randomGuid = Guid.NewGuid();
            await _fixture.Persistence.InsertList(exampleGenresList);

            
            var (response, output) = await _fixture.APIClient
                .Get<ProblemDetails>($"/api/genres/{randomGuid}");

            
            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output.Title = "One or more validation errors occurred.";
            output.Status = (int)StatusCodes.Status404NotFound;
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Genre '{randomGuid}' not found.");
        }

        [Fact(DisplayName = nameof(GetGenreWithRelations))]
        [Trait("EndToEnd/API", "Genre/GetGenreById - Endpoints")]
        public async Task GetGenreWithRelations()
        {
            // cadastrar uma lista de genres na persistencia
            var dbContext = _fixture.CreateDbContext();
            var exampleGenresList = _fixture.GetExampleListGenres();
            var targetGenre = exampleGenresList[5];
            var exampleCategories = _fixture.GetExampleCategoriesList(10);
            var random = new Random();
            exampleGenresList.ForEach(genre =>
            {
                int relationsCount = random.Next(2, exampleCategories.Count - 1);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
                    var selectedCategory = exampleCategories[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selectedCategory.Id))
                        genre.AddCategory(selectedCategory.Id);
                }
            });

            List<GenresCategories> genresCategories = new List<GenresCategories>();

            exampleGenresList.ForEach(
                genre => genre.Categories.ToList().ForEach(
                    categoryId => genresCategories.Add(
                        new GenresCategories(categoryId, genre.Id)
                    )
                )
            );
            await _fixture.Persistence.InsertList(exampleGenresList);
            await _fixture.CategoryPersistence.InsertList(exampleCategories);
            await _fixture.Persistence.InsertGenresCategoriesRelationsList(genresCategories);

            // busca por um genre especifico por rest
            var (response, output) = await _fixture.APIClient
                .Get<APIResponse<GenreModelOutput>>($"/api/genres/{targetGenre.Id}");

            // testa e ve se o item é o que a gente espera\
            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output!.Data.Should().NotBeNull();
            output.Data.Id.Should().Be(targetGenre.Id);
            output.Data.Name.Should().Be(targetGenre.Name);
            output.Data.IsActive.Should().Be(targetGenre.IsActive);
            List<Guid> relatedCategoriesIds = output.Data.Categories.Select(relation => relation.Id).ToList();
            relatedCategoriesIds.Should().BeEquivalentTo(targetGenre.Categories);
        }
    }
}
