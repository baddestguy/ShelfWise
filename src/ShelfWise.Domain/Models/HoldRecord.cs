using System;

namespace ShelfWise.Domain.Models
{
    public class HoldRecord
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? NotifiedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public Book Book { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
