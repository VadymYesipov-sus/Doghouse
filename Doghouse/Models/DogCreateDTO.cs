using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Doghouse.Models
{
    public class DogCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must have at least 3 letters")]
        public string Name { get; set; }

        [Required]
        public string Color { get; set; }
        [Required]
        public int TailLength { get; set; }

        [Range(1, 100)]
        public int Weight { get; set; }
    }
}
