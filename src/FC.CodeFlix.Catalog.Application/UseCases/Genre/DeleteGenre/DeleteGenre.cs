using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Repository;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre
{
    public class DeleteGenre : IDeleteGenre
    {
        private IGenreRepository _genreRepository;
        private IUnitOfWork _unitOfWork;

        public DeleteGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork)
        {
            _genreRepository = genreRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Unit> Handle(DeleteGenreInput request, CancellationToken cancellationToken)
        {
            var genre = await _genreRepository.Get(request.Id, cancellationToken);
            NotFoundException.ThrowIfNull(genre, $"Genre '{request.Id}' not found.");
            await _genreRepository.Delete(genre, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return await Task.FromResult(Unit.Value);
        }
    }
}
