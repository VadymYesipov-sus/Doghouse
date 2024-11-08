using Doghouse.Filters;
using Doghouse.Helpers;
using Doghouse.Interfaces;
using Doghouse.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Doghouse.Controllers
{
    [ApiController]
    [Route("dog")]
    [ServiceFilter(typeof(GeneralExceptionFilter))]
    public class DogController : Controller
    {

        private readonly IDogService _dogService;

        public DogController(IDogService dogService)
        {
            _dogService = dogService;
        }

        [HttpGet]
        [ServiceFilter(typeof(QueryValidationFilterAttribute))]
        public async Task<IActionResult> GetAllDogsAsync([FromQuery] DogQueryObject query)
        {
            var paginatedDogs = await _dogService.GetAllDogsAsync(query);

            return Ok(paginatedDogs);
        }

        [HttpPost]
        [ServiceFilter(typeof(DogCreationValidationFilter))]
        public async Task<IActionResult> CreateDogAsync([FromBody] DogCreateDto dogCreateDto)
        {
            var newDog = await _dogService.CreateDogAsync(dogCreateDto);

            return Ok(newDog);
        }


    }
}
