using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre
{
    public class DeleteGenreInput : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public DeleteGenreInput(Guid id)
            => Id = id;
    }
}
