using Doghouse.Helpers;
using Doghouse.Interfaces;
using Doghouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Doghouse.Services
{
    public class QueryValidationFilterAttribute : IAsyncActionFilter
    {
        private readonly IDogRepository _dogRepository;

        public QueryValidationFilterAttribute(IDogRepository dogRepository)
        {
            _dogRepository = dogRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("query", out var queryObj) && queryObj is DogQueryObject query)
            {
                if (!query.IsValid())
                {
                    context.Result = new BadRequestObjectResult("Invalid sorting attribute or order.");
                    return;
                }

                var resultContext = await next();

                if (resultContext.Result is OkObjectResult okResult)
                {
                    var paginatedDogs = okResult.Value as PaginatedList<Dog>;
                    if (paginatedDogs == null || !paginatedDogs.Items.Any())
                    {
                        okResult.Value = new PaginatedList<Dog>(new List<Dog>(), 0, 0, 0);
                    }
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("Invalid query parameters.");
            }
        }


    }
}
