namespace AICommunicationService.BLL.Dtos
{
    public class DocumentResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public bool? IsReadyToUse { get; set; } = null;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}
