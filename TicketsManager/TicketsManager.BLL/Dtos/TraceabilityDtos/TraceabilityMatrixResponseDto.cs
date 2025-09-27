namespace TicketsManager.BLL.Dtos.TraceabilityDtos;

public class TraceabilityMatrixResponseDto
{
    public IEnumerable<TraceabilityStoryDto> Stories { get; set; } = new List<TraceabilityStoryDto>();
}