namespace ShelfWise.Api.Models
{
    public class AiBookMatchDto
    {
        public BookResponseDto Book { get; set; } = new();
        public double Score { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
