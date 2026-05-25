using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.API.APIModels.Video;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FC.CodeFlix.Catalog.EndToEndTests.API.Video.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Video.CreateVideo
{
    [Collection(nameof(VideoBaseFixture))]
    public class CreateVideoAPITest : IDisposable
    {
        private readonly VideoBaseFixture _fixture;

        public CreateVideoAPITest(VideoBaseFixture fixture)
            => _fixture = fixture;

        public void Dispose()
            => _fixture.CleanPersistence();

        [Fact(DisplayName = nameof(CreateBasicVideo))]
        [Trait("EndToEnd/API", "Video/Create - Endpoints")]
        public async Task CreateBasicVideo()
        {
            CreateVideoAPIInput input = _fixture.GetBasicCreateVideoInput();

            var (response, output) = await 
                _fixture.APIClient.Post<APIResponse<VideoModelOutput>>("/api/videos", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
            output.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Data.Id.Should().NotBeEmpty();
            output.Data.Title.Should().Be(input.Title);
            output.Data.Description.Should().Be(input.Description);
            output.Data.YearLaunched.Should().Be(input.YearLaunched);
            output.Data.Opened.Should().Be(input.Opened);
            output.Data.Duration.Should().Be(input.Duration);
            output.Data.Rating.Should().Be(input.Rating.ToStringSignal());

            var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
            videoFromDb.Should().NotBeNull();
            videoFromDb.Id.Should().NotBeEmpty();
            videoFromDb.Title.Should().Be(input.Title);
            videoFromDb.Description.Should().Be(input.Description);
            videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
            videoFromDb.Opened.Should().Be(input.Opened);
            videoFromDb.Duration.Should().Be(input.Duration);
            videoFromDb.Rating.Should().Be(input.Rating);
        }
    }
}
