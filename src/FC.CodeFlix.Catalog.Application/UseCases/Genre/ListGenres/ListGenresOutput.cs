﻿using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.ListGenres
{
    public class ListGenresOutput : PaginatedListOutput<GenreModelOutput>
    {
        public ListGenresOutput(
            int page,
            int perPage,
            int total,
            IReadOnlyList<GenreModelOutput> items) : base(page, perPage, total, items)
        { }

        public static ListGenresOutput FromSearchOutput(
            SearchOutput<DomainEntity.Genre> searchOutput)
        {
            return new ListGenresOutput(
                page: searchOutput.CurrentPage,
                perPage: searchOutput.PerPage,
                total: searchOutput.Total,
                items: searchOutput.Items
                .Select(GenreModelOutput.FromGenre)
                .ToList());
        }
    }
}
