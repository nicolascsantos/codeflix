﻿using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory
{
    interface IUpdateCategory : IRequestHandler<UpdateCategoryInput, CategoryModelOutput>
    {
    }
}
