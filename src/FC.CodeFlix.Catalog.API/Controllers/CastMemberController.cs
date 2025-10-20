using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;
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
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new GetCastMemberInput(id), cancellationToken);
            return Ok(new APIResponse<CastMemberModelOutput>(output));
        }
    }
}
