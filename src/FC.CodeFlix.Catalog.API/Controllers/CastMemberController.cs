using FC.CodeFlix.Catalog.API.APIModels.CastMember;
using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.CodeFlix.Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CastMemberController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CastMemberController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<CastMemberModelOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new GetCastMemberInput(id), cancellationToken);
            return Ok(new APIResponse<CastMemberModelOutput>(output));
        }

        [HttpDelete("{id::guid}")]
        [ProducesResponseType(typeof(APIResponse<CastMemberModelOutput>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new DeleteCastMemberInput(id), cancellationToken);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(201, StatusCode = StatusCodes.Status201Created, Type = typeof(APIResponse<CastMemberModelOutput>))]
        [ProducesResponseType(400, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(422, StatusCode = StatusCodes.Status422UnprocessableEntity, Type = typeof(CastMemberModelOutput))]
        public async Task<IActionResult> Create([FromBody] CreateCastMemberInput input, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(input, cancellationToken);
            return CreatedAtAction(nameof(Create), new { output.Id }, new APIResponse<CastMemberModelOutput>(output));
        }

        [HttpPut("{id::guid}")]
        [ProducesResponseType(200, StatusCode = StatusCodes.Status200OK, Type = typeof(APIResponse<CastMemberModelOutput>))]
        [ProducesResponseType(400, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(404, StatusCode = StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(423, StatusCode = StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Update([FromBody] UpdateCastMemberAPIInput input, [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new UpdateCastMemberInput(id, input.Name, input.Type), cancellationToken);
            return Ok(new APIResponse<CastMemberModelOutput>(output));
        }
    }
}
