using System;
using System.Collections.Generic;

namespace ShelfWise.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
        public ICollection<HoldRecord> HoldRecords { get; set; } = new List<HoldRecord>();
    }
}
