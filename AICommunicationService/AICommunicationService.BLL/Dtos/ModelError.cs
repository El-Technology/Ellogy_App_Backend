namespace AICommunicationService.BLL.Dtos
{
    public class Error
    {
        public string? Message { get; set; }
        public string? Type { get; set; }
        public object? Param { get; set; }
        public object? Code { get; set; }
    }

    public class ModelError
    {
        public Error? Error { get; set; }
    }
}
