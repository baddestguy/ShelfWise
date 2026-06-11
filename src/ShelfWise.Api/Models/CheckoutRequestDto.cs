namespace ShelfWise.Api.Models
{
    public class CheckoutRequestDto
    {
        public int UserId { get; set; }
        public int DueDays { get; set; } = 14;
    }
}
