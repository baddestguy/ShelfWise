using System.ComponentModel.DataAnnotations;

namespace ShelfWise.Api.Models
{
    public class UpdateBookDto
    {
        public string? Title { get; set; }

        public string? Author { get; set; }

        public string? Category { get; set; }

        public string? Genre { get; set; }

        [Range(0, int.MaxValue)]
        public int? TotalCopies { get; set; }

        [Range(0, int.MaxValue)]
        public int? OnHold { get; set; }
    }
}
