using Doghouse.Data;
using Doghouse.Helpers;
using Doghouse.Interfaces;
using Doghouse.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doghouse.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly ApplicationDbContext _context;

        public DogRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Dog> CreateAsync(Dog dog)
        {
            _context.Add(dog);
            await _context.SaveChangesAsync();

            return dog;
        }

        public async Task<PaginatedList<Dog>> GetAllAsync(DogQueryObject query)
        {
            IQueryable<Dog> dogQuery = _context.Dogs.AsQueryable();

            dogQuery = query.ApplySorting(dogQuery);

            var totalCount = await dogQuery.CountAsync();

            var dogs = await dogQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PaginatedList<Dog>(dogs, totalCount, query.PageNumber, query.PageSize);
        }

        public async Task<Dog?> GetByNameAsync(string name)
        {
            return await _context.Dogs.FirstOrDefaultAsync(d => d.Name == name);
        }

    }
}
