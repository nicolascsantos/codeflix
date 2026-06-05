using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.API.APIModels.Video;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
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
    }
}
