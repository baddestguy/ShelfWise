using ShelfWise.Domain.Models;

namespace ShelfWise.Api.Models
{
    public class BookResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
        public int CheckedOutCopies { get; set; }
        public int AvailableCopies { get; set; }

        public static BookResponseDto FromInventory(BookInventoryItem item)
        {
            return new BookResponseDto
            {
                Id = item.Id,
                Title = item.Title,
                Author = item.Author,
                Category = item.Category.ToString(),
                Genre = item.Genre,
                TotalCopies = item.TotalCopies,
                CheckedOutCopies = item.CheckedOutCopies,
                AvailableCopies = item.AvailableCopies
            };
        }
    }
}
