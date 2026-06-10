namespace ShelfWise.Domain.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public Category Category { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int Available { get; set; }
        public int OnHold { get; set; }
        public int TotalCopies { get; set; }
    }
}

