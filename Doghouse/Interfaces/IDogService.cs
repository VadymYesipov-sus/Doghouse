using Doghouse.Helpers;
using Doghouse.Models;

namespace Doghouse.Interfaces
{
    public interface IDogService
    {
        Task<PaginatedList<Dog>> GetAllDogsAsync(DogQueryObject query);
        Task<Dog> CreateDogAsync(DogCreateDto dogCreateDto);

    }
}
