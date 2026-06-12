namespace ShelfWise.Api.Models
{
    public class AiBookSearchResponseDto
    {
        public string Query { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public IEnumerable<AiBookMatchDto> Matches { get; set; } = Array.Empty<AiBookMatchDto>();
    }
}
