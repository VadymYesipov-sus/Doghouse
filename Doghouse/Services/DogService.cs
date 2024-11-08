using Doghouse.Helpers;
using Doghouse.Interfaces;
using Doghouse.Models;

namespace Doghouse.Services
{
    public class DogService : IDogService
    {
        private readonly IDogRepository _dogRepository;

        public DogService(IDogRepository dogRepository)
        {
            _dogRepository = dogRepository;
        }

        public async Task<PaginatedList<Dog>> GetAllDogsAsync(DogQueryObject query)
        {
            return await _dogRepository.GetAllAsync(query);
        }

        public async Task<Dog> CreateDogAsync(DogCreateDto dogCreateDto)
        {
            var newDog = new Dog
            {
                Name = dogCreateDto.Name,
                Color = dogCreateDto.Color,
                TailLength = dogCreateDto.TailLength,
                Weight = dogCreateDto.Weight
            };
            await _dogRepository.CreateAsync(newDog);
            return newDog;
        }

    }
}
