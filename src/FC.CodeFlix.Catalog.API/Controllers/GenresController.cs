using FC.CodeFlix.Catalog.API.APIModels.Genre;
using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.CodeFlix.Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GenresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new GetGenreInput(id), cancellationToken);
            return Ok(new APIResponse<GenreModelOutput>(output));
        }


        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<GenreModelOutput>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new DeleteGenreInput(id), cancellationToken);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(201, StatusCode = StatusCodes.Status201Created, Type = typeof(APIResponse<GenreModelOutput>))]
        [ProducesResponseType(400, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(422, StatusCode = StatusCodes.Status422UnprocessableEntity, Type = typeof(GenreModelOutput))]
        public async Task<IActionResult> Create([FromBody] CreateGenreInput input, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(input, cancellationToken);
            return CreatedAtAction(nameof(Create), new { output.Id }, new APIResponse<GenreModelOutput>(output));
        }

        [HttpPut("{id::guid}")]
        [ProducesResponseType(200, StatusCode = StatusCodes.Status200OK, Type = typeof(APIResponse<GenreModelOutput>))]
        [ProducesResponseType(400, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(422, StatusCode = StatusCodes.Status422UnprocessableEntity, Type = typeof(GenreModelOutput))]
        public async Task<IActionResult> Update(
            [FromBody] UpdateGenreAPIInput input,
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var output = await _mediator.Send(
                new UpdateGenreInput(id,
                input.Name,
                input.IsActive,
                input.CategoriesIds),
                cancellationToken
            );

            return Ok(new APIResponse<GenreModelOutput>(output));
        }
    }
}
