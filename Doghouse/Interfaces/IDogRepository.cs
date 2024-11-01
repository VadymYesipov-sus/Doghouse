using Doghouse.Helpers;
using Doghouse.Models;

namespace Doghouse.Interfaces
{
    public interface IDogRepository
    {
        Task<PaginatedList<Dog>> GetAllAsync(DogQueryObject query);
        Task<Dog> CreateAsync(Dog dog);
        Task<Dog?> GetByNameAsync(string name);
    }
}
