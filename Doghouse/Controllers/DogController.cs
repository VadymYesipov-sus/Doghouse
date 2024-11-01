using Doghouse.Helpers;
using Doghouse.Interfaces;
using Doghouse.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Doghouse.Controllers
{
    public class DogController : Controller
    {
        private readonly IDogRepository _dogRepository;

        public DogController(IDogRepository dogRepository)
        {
            _dogRepository = dogRepository;
        }

        [HttpGet]
        [Route("dogs")]
        public async Task<IActionResult> GetAllDogsAsync([FromQuery] DogQueryObject query)
        {
            if (!query.IsValid())
            {
                return BadRequest("Invalid sorting attribute or order.");
            }

            try
            {
                var paginatedDogs = await _dogRepository.GetAllAsync(query);

                if (paginatedDogs == null || !paginatedDogs.Items.Any())
                {
                    return NotFound("No dogs found.");
                }

                return Ok(paginatedDogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("dog")]
        public async Task<IActionResult> CreateDogAsync(DogCreateDTO dogCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid JSON is passed in a request body or missing required fields.");
            }

            var existingDog = await _dogRepository.GetByNameAsync(dogCreateDTO.Name);
            if (existingDog != null)
            {
                return Conflict($"A dog with the name '{dogCreateDTO.Name}' already exists in DB.");
            }

            if (dogCreateDTO.TailLength < 0)
            {
                return BadRequest("Tail length must be a non-negative number.");
            }

            Dog newDog = new Dog
            {
                Name = dogCreateDTO.Name,
                Color = dogCreateDTO.Color,
                TailLength = dogCreateDTO.TailLength,
                Weight = dogCreateDTO.Weight,
            };

            try
            {
                await _dogRepository.CreateAsync(newDog);
                return Ok(newDog);
            }
            catch (JsonException)
            {
                return BadRequest("Invalid JSON is passed in a request body.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error during dog creation: {ex.Message}");
            }
        }
    }
}
