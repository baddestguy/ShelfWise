namespace ShelfWise.Domain.Models
{
    public class BookInventoryItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public Category Category { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
        public int CheckedOutCopies { get; set; }
        public int AvailableCopies { get; set; }
    }
}
