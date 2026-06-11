using System.ComponentModel.DataAnnotations;
using ShelfWise.Domain.Models;

namespace ShelfWise.Api.Models
{
    public class CreateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = "NonFiction";

        [StringLength(100)]
        public string Genre { get; set; } = string.Empty;

        [Range(0, 1000)]
        public int TotalCopies { get; set; } = 1;
    }
}
