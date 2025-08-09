using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.Common
{
    public class GenreModelOutput
    {
        public GenreModelOutput(
            Guid id,
            string name,
            bool isActive,
            DateTime createdAt,
            IReadOnlyList<GenreModelOuputCategory> categories
        )
        {
            Id = id;
            Name = name;
            IsActive = isActive;
            CreatedAt = createdAt;
            Categories = categories;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public IReadOnlyList<GenreModelOuputCategory> Categories { get; set; }

        public static GenreModelOutput FromGenre(DomainEntity.Genre genre)
            => new GenreModelOutput(genre.Id, genre.Name, genre.IsActive, genre.CreatedAt, genre.Categories.Select(
                categoryId => new GenreModelOuputCategory(categoryId)
            ).ToList()
            .AsReadOnly());
    }

    public class GenreModelOuputCategory
    {
        public GenreModelOuputCategory(Guid id, string? name = null)
            => (Id, Name) = (id, name);

        public Guid Id { get; set; }

        public string? Name { get; set; }
    }
}
