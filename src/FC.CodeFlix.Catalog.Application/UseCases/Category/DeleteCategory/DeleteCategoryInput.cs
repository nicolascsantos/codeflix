using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory
{
    public class DeleteCategoryInput : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public DeleteCategoryInput(Guid id) => Id = id;
    }
}
