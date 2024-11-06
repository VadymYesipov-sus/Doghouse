using Doghouse.Helpers;
using Doghouse.Interfaces;
using Doghouse.Models;
using Doghouse.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Doghouse.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(GeneralExceptionFilter))]
    public class DogController : Controller
    {
        private readonly IDogRepository _dogRepository;

        public DogController(IDogRepository dogRepository)
        {
            _dogRepository = dogRepository;
        }

        [HttpGet]
        [Route("dog")]
        [ServiceFilter(typeof(QueryValidationFilterAttribute))]
        public async Task<IActionResult> GetAllDogsAsync([FromQuery] DogQueryObject query)
        {
            var paginatedDogs = await _dogRepository.GetAllAsync(query);

            return Ok(paginatedDogs);
        }

        [HttpPost]
        [Route("dog")]
        [ServiceFilter(typeof(DogCreationValidationFilter))]
        public async Task<IActionResult> CreateDogAsync([FromBody] DogCreateDto dogCreateDto)
        {
            Dog newDog = new Dog
            {
                Name = dogCreateDto.Name,
                Color = dogCreateDto.Color,
                TailLength = dogCreateDto.TailLength,
                Weight = dogCreateDto.Weight,
            };

            await _dogRepository.CreateAsync(newDog);
            return Ok(newDog);
        }


    }
}
