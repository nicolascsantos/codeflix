using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.CodeFlix.Catalog.API.APIModels.Response;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.DeleteGenre
{
    [Collection(nameof(DeleteGenreAPITestFixture))]
    public class DeleteGenreAPITest
    {
        private readonly DeleteGenreAPITestFixture _fixture;

        public DeleteGenreAPITest(DeleteGenreAPITestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteGenre))]
        [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]
        public async Task DeleteGenre()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenreList = _fixture.GetExampleListGenres();
            await _fixture.Persistence.InsertList(exampleGenreList);
            var genreToDelete = exampleGenreList[5];

            var (response, output) = await _fixture.APIClient
                .Delete<APIResponse<object>>($"/api/genres/{genreToDelete.Id}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
            output.Should().BeNull();
            var persistenceGenre = await _fixture.Persistence.GetById(genreToDelete.Id);
            persistenceGenre.Should().BeNull();
        }

        [Fact(DisplayName = nameof(WhenNotFound404))]
        [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]
        public async Task WhenNotFound404()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenreList = _fixture.GetExampleListGenres();
            await _fixture.Persistence.InsertList(exampleGenreList);
            var randomGuid = Guid.NewGuid();
            var (response, output) = await _fixture.APIClient
                .Delete<ProblemDetails>($"/api/genres/{randomGuid}");


            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output.Title = "One or more validation errors occurred.";
            output.Status = (int)StatusCodes.Status404NotFound;
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Genre '{randomGuid}' not found.");
        }

        [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
        [Trait("EndToEnd/API", "Genre/Delete - Endpoints")]
        public async Task DeleteGenreWithRelations()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleGenreList = _fixture.GetExampleListGenres();
            var exampleCategoriesList = _fixture.GetExampleCategoriesList();
            var genreToDelete = exampleGenreList[5];

            var random = new Random();
            exampleGenreList.ForEach(genre =>
            {
                int relationsCount = random.Next(2, exampleCategoriesList.Count - 1);
                for (int i = 0; i < relationsCount; i++)
                {
                    var selectedCategoryIndex = random.Next(0, exampleCategoriesList.Count - 1);
                    var selectedCategory = exampleCategoriesList[selectedCategoryIndex];
                    if (!genre.Categories.Contains(selectedCategory.Id))
                        genre.AddCategory(selectedCategory.Id);
                }
            });

            List<GenresCategories> genresCategories = new List<GenresCategories>();

            exampleGenreList.ForEach(
                genre => genre.Categories.ToList().ForEach(
                    categoryId => genresCategories.Add(
                        new GenresCategories(categoryId, genre.Id)
                    )
                )
            );

            await _fixture.Persistence.InsertList(exampleGenreList);
            await _fixture.CategoryPersistence.InsertList(exampleCategoriesList);
            await _fixture.Persistence.InsertGenresCategoriesRelationsList(genresCategories);

            var (response, output) = await _fixture.APIClient
                .Delete<APIResponse<object>>($"/api/genres/{genreToDelete.Id}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
            output.Should().BeNull();
            var persistenceGenre = await _fixture.Persistence.GetById(genreToDelete.Id);
            persistenceGenre.Should().BeNull();
            List<GenresCategories> relations = await _fixture.Persistence
                .GetGenresCategoriesRelationsByGenreId(genreToDelete.Id);
            relations.Should().HaveCount(0);
        }
    }
}
