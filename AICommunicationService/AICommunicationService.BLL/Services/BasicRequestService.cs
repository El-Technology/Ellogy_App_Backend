using AICommunicationService.BLL.Exceptions;
using AICommunicationService.Common.Models.AIRequest;
using System.Collections;
using System.Net;

namespace AICommunicationService.BLL.Services;
public abstract class BasicRequestService
{
    protected List<object> GetMessages(MessageRequest request)
    {
        var messages = new List<object>()
        {
             new { role = "system", content = request.Template }
        };

        if (!string.IsNullOrEmpty(request.Context))
            messages.Add(new { role = "system", content = request.Context });

        if (request.ConversationHistory is not null)
            foreach (DictionaryEntry s in request.ConversationHistory)
            {
                messages.AddRange(new List<object>
                {
                    new { role = "user", content = s.Key.ToString() },
                    new { role = "assistant", content = s.Value?.ToString() }
                });
            }

        messages.Add(new { role = "user", content = request.UserInput });

        return messages;
    }

    protected void ValidateResponse(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                throw new GptModelException($"Model error, try to replace with another one");

            case HttpStatusCode.TooManyRequests:
                response.Headers.TryGetValues("retry-after", out var values);
                throw new ToManyRequestsException(values?.FirstOrDefault());

            default:
                response.EnsureSuccessStatusCode();
                break;
        }
    }
}
