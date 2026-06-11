using System;

namespace ShelfWise.Domain.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckedOutAt { get; set; }
        public DateTime DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }

        public Book Book { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
