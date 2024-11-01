using Doghouse.Models;
using Microsoft.EntityFrameworkCore;

namespace Doghouse.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {

        }


        public DbSet<Dog> Dogs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
