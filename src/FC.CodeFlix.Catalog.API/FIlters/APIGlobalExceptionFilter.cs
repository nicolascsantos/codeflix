using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FC.CodeFlix.Catalog.API.FIlters
{
    public class APIGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _env;

        public APIGlobalExceptionFilter(IHostEnvironment env)
            => _env = env;


        public void OnException(ExceptionContext context)
        {
            var details = new ProblemDetails();
            var exception = context.Exception;
            if (_env.IsDevelopment())
                details.Extensions.Add("StackTrace", exception.StackTrace);
            if (exception is EntityValidationException)
            {
                var ex = exception as EntityValidationException;
                details.Title = "One or more validation errors occurred.";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Detail = ex!.Message;
                details.Type = "UnprocessableEntity";
            }
            else if (exception is NotFoundException)
            {
                var ex = exception as NotFoundException;
                details.Title = "One or more validation errors occurred.";
                details.Status = StatusCodes.Status404NotFound;
                details.Detail = ex!.Message;
                details.Type = "NotFound";
            }
            else if (exception is RelatedAggregateException)
            {
                var ex = exception as RelatedAggregateException;
                details.Title = "Invalid Related Aggregate.";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Detail = ex!.Message;
                details.Type = "RelatedAggregate";
            }
            else
            {
                details.Title = "An unexpected error occured.";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Type = "UnexpectedError";
                details.Detail = exception.Message;
            }
            context.HttpContext.Response.StatusCode = (int)details.Status!;
            context.Result = new ObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
