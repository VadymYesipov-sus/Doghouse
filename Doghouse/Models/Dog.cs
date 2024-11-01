using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doghouse.Models
{
    public class Dog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int TailLength { get; set; }
        public int Weight { get; set; }
    }
}
