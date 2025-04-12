﻿using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.ListGenres
{
    public class ListGenresInput : PaginatedListInput, IRequest<ListGenresOutput>
    {
        public ListGenresInput(
            int page = 1,
            int perPage = 15,
            string search = "",
            string sort = "",
            SearchOrder dir = SearchOrder.Asc)
            : base(page, perPage, search, sort, dir)
        { }
        public ListGenresInput()
            : base(1, 15, "", "", SearchOrder.Asc)
        { }
    }
}
