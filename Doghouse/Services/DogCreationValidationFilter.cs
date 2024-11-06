using Doghouse.Interfaces;
using Doghouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace Doghouse.Services
{
    public class DogCreationValidationFilter : IAsyncActionFilter
    {
        private readonly IDogRepository _dogRepository;
        private readonly ILogger<DogCreationValidationFilter> _logger;

        public DogCreationValidationFilter(IDogRepository dogRepository, ILogger<DogCreationValidationFilter> logger)
        {
            _dogRepository = dogRepository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
                return;
            }

            if (context.ActionArguments.TryGetValue("dogCreateDto", out var dto) && dto is DogCreateDto dogCreateDto)
            {
                var existingDog = await _dogRepository.GetByNameAsync(dogCreateDto.Name);
                if (existingDog != null)
                {
                    context.Result = new ConflictObjectResult($"A dog with the name '{dogCreateDto.Name}' already exists in DB.");
                    return;
                }

                if (dogCreateDto.TailLength < 0)
                {
                    context.Result = new BadRequestObjectResult("Tail length must be a non-negative number.");
                    return;
                }
            }

            try
            {
                var result = await next();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON passed in request body");
                context.Result = new BadRequestObjectResult("Invalid JSON in request body. >:( ");
            }
        }

    }
}
