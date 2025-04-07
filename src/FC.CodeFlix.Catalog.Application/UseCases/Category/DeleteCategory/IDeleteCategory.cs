using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory
{
    interface IDeleteCategory : IRequestHandler<DeleteCategoryInput, Unit>
    {
    }
}
