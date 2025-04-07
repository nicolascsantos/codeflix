using FC.CodeFlix.Catalog.API.APIModels.Category;
using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.CodeFlix.Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(201, StatusCode = StatusCodes.Status201Created, Type = typeof(APIResponse<CategoryModelOutput>))]
        [ProducesResponseType(400, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(422, StatusCode = StatusCodes.Status422UnprocessableEntity, Type = typeof(CategoryModelOutput))]
        public async Task<IActionResult> Create([FromBody] CreateCategoryInput input, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(input, cancellationToken);
            return CreatedAtAction(nameof(Create), new { output.Id }, new APIResponse<CategoryModelOutput>(output));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(200, StatusCode = StatusCodes.Status200OK, Type = typeof(CategoryModelOutput))]
        [ProducesResponseType(200, StatusCode = StatusCodes.Status200OK, Type = typeof(APIResponse<CategoryModelOutput>))]
        [ProducesResponseType(404, StatusCode = StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new GetCategoryInput(id), cancellationToken);
            return Ok(new APIResponse<CategoryModelOutput>(output));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204, StatusCode = StatusCodes.Status204NoContent, Type = typeof(object))]
        [ProducesResponseType(404, StatusCode = StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new DeleteCategoryInput(id), cancellationToken);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(200, StatusCode = StatusCodes.Status200OK, Type = typeof(APIResponse<CategoryModelOutput>))]
        [ProducesResponseType(400, StatusCode = StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(404, StatusCode = StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Update(
            [FromBody] UpdateCategoryAPIInput apiInput,
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var input = new UpdateCategoryInput(
                id,
                apiInput.Name,
                apiInput.Description,
                apiInput.IsActive
            );
            var output = await _mediator.Send(input, cancellationToken);
            return Ok(new APIResponse<CategoryModelOutput>(output));
        }

        [HttpGet]
        [ProducesResponseType(200, StatusCode = StatusCodes.Status200OK, Type = typeof(APIResponseList<CategoryModelOutput>))]
        public async Task<IActionResult> List(
            CancellationToken cancellationToken,
            [FromQuery] int? page = null,
            [FromQuery(Name = "per_page")] int? perPage = null,
            [FromQuery] string? search = "",
            [FromQuery] string? sort = "",
            [FromQuery] SearchOrder? dir = null
        )
        {
            var input = new ListCategoriesInput();
            if (page is not null) input.Page = page.Value;
            if (perPage is not null) input.PerPage = perPage.Value;
            if (!string.IsNullOrEmpty(search)) input.Search = search;
            if (!string.IsNullOrEmpty(sort)) input.Sort = sort;
            if (dir is not null) input.Dir = dir.Value;

            var output = await _mediator.Send(input, cancellationToken);
            return Ok(new APIResponseList<CategoryModelOutput>(output));
        }
    }
}
