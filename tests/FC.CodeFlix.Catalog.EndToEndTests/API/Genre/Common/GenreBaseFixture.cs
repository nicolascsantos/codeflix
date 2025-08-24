﻿using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.EndToEndTests.Base;
using FC.CodeFlix.Catalog.EndToEndTests.API.Category.Common;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.Common
{
    public class GenreBaseFixture : BaseFixture
    {
        public GenrePersistence Persistence { get; set; }

        public CategoryPersistence CategoryPersistence { get; set; }

        public GenreBaseFixture() : base()
        {
            var dbContext = CreateDbContext();
            Persistence = new GenrePersistence(dbContext);
            CategoryPersistence = new CategoryPersistence(dbContext);
        }

        public string GetValidGenreName()
            => Faker.Commerce.Categories(1)[0];

        public bool GetRandomBoolean() => new Random().NextDouble() < 0.5;

        public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null, string? name = null)
        {
            var genre = new DomainEntity.Genre(name ?? GetValidGenreName(), isActive ?? GetRandomBoolean());
            categoriesIds?.ForEach(genre.AddCategory);
            return genre;
        }

        public List<DomainEntity.Genre> GetExampleListGenres(int count = 10)
            => Enumerable.Range(1, count).Select(_ => GetExampleGenre()).ToList();

        public List<DomainEntity.Category> GetExampleCategoriesList(int length = 10) => Enumerable.Range(1, length).Select(
           _ => GetExampleCategory())
                   .ToList();

        public DomainEntity.Category GetExampleCategory()
        {
            return new DomainEntity.Category(
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                GetRandomBoolean()
            );
        }

        public string GetValidCategoryName()
        {
            var categoryName = "";
            while (categoryName.Length < 3)
                categoryName = Faker.Commerce.Categories(1)[0];
            if (categoryName.Length > 255)
                categoryName = categoryName[..255];
            return categoryName;
        }

        public string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            while (categoryDescription.Length > 10_000)
                categoryDescription = categoryDescription[..10_000];
            return categoryDescription;
        }
    }
}
