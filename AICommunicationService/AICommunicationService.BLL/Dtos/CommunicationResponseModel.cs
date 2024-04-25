using AICommunicationService.Common.Models.GptResponseModel;

namespace AICommunicationService.BLL.Dtos;

public class CommunicationResponseModel
{
    public string? Content { get; set; }
    public Usage? Usage { get; set; }
}
