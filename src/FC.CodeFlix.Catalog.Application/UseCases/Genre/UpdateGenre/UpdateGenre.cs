using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre
{
    public class UpdateGenre : IUpdateGenre
    {
        private IGenreRepository _genreRepository;
        private IUnitOfWork _unitOfWork;
        private ICategoryRepository _categoryRepository;

        public UpdateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
        {
            _genreRepository = genreRepository;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<GenreModelOutput> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
        {
            var genre = await _genreRepository.Get(request.Id, cancellationToken);
            genre.Update(request.Name);
            if (request.IsActive is null) request.IsActive = true;
            if (request.IsActive != genre.IsActive)
                if ((bool)request.IsActive!) genre.Activate();
                else genre.Deactivate();
            await _genreRepository.Update(genre, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return GenreModelOutput.FromGenre(genre);
        }
    }
}
