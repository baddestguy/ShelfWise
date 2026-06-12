using System.ComponentModel.DataAnnotations;

namespace ShelfWise.Api.Models
{
    public class AiBookSearchRequestDto
    {
        [Required]
        [StringLength(500)]
        public string Query { get; set; } = string.Empty;
    }
}
