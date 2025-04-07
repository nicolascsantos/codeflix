using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre
{
    public class UpdateGenreInput : IRequest<GenreModelOutput>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool? IsActive { get; set; }
        public UpdateGenreInput(string name, bool? isActive = null)
        {
            Name = name;
            IsActive = isActive;
        }
    }
}
