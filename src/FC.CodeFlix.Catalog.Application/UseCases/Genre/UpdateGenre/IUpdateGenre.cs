﻿using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre
{
    interface IUpdateGenre : IRequestHandler<UpdateGenreInput, GenreModelOutput>
    {
    }
}
