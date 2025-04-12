using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre
{
    interface IGetGenre : IRequestHandler<GetGenreInput, GenreModelOutput>
    {
    }
}
