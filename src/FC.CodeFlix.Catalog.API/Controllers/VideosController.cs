using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.API.APIModels.Video;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Video.GetVideo;
using FC.CodeFlix.Catalog.Application.UseCases.Video.ListVideos;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.CodeFlix.Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VideosController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<VideoModelOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new GetVideoInput(id), cancellationToken);
            return Ok(new APIResponse<VideoModelOutput>(output));
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(APIResponse<VideoModelOutput>), StatusCode = StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(422, Type = typeof(ProblemDetails), StatusCode = StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateVideo([FromBody] CreateVideoAPIInput request)
        {
            var input = request.ToCreateVideoInput();
            var output = await _mediator.Send(input);
            return CreatedAtAction(nameof(CreateVideo), new { id = output.Id }, new APIResponse<VideoModelOutput>(output));
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(APIResponseList<VideoModelOutput>), StatusCode = StatusCodes.Status200OK)]
        public async Task<IActionResult> List(
            CancellationToken cancellationToken,
            [FromQuery] int? page = null,
            [FromQuery(Name = "per_page")] int? perPage = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sort = null,
            [FromQuery] SearchOrder? dir = null
        )
        {
            var input = new ListVideosInput();
            if (page is not null) input.Page = page.Value;
            if (perPage is not null) input.PerPage = perPage.Value;
            if (!string.IsNullOrWhiteSpace(search)) input.Search = search;
            if (!string.IsNullOrWhiteSpace(sort)) input.Sort = sort;
            if (dir is not null) input.Dir = dir.Value;
            var output = await _mediator.Send(input, cancellationToken);
            return Ok(new APIResponseList<VideoModelOutput>(output));
        }
    }
}
