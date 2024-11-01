using Doghouse.Models;
using System.ComponentModel.DataAnnotations;

namespace Doghouse.Helpers
{
    public class DogQueryObject
    {
        private const int MaxPageSize = 50;
        private const int DefaultPageSize = 10;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = DefaultPageSize;
        [Range(1, MaxPageSize)]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string Attribute { get; set; }

        public string Order { get; set; } = "asc";

        public bool IsValid()
        {

            var allowedAttributes = new[] { "weight", "taillength", "name", "color" };
            if (!string.IsNullOrEmpty(Attribute))
            {
                var lowerAttribute = Attribute.ToLower();
                if (!allowedAttributes.Contains(lowerAttribute))
                {
                    return false;
                }
            }

            var allowedOrders = new[] { "asc", "desc" };
            if (!string.IsNullOrEmpty(Order))
            {
                var lowerOrder = Order.ToLower();
                if (!allowedOrders.Contains(lowerOrder))
                {
                    return false;
                }
            }

            return true;
        }

        public IQueryable<Dog> ApplySorting(IQueryable<Dog> query)
        {
            switch (Attribute?.ToLower())
            {
                case "name":
                    return Order.ToLower() == "desc" ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name);
                case "color":
                    return Order.ToLower() == "desc" ? query.OrderByDescending(d => d.Color) : query.OrderBy(d => d.Color);
                case "taillength":
                    return Order.ToLower() == "desc" ? query.OrderByDescending(d => d.TailLength) : query.OrderBy(d => d.TailLength);
                case "weight":
                    return Order.ToLower() == "desc" ? query.OrderByDescending(d => d.Weight) : query.OrderBy(d => d.Weight);
                default:
                    return query.OrderBy(d => d.Name);
            }
        }

    }
}
