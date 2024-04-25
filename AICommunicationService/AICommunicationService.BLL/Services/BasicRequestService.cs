using AICommunicationService.Common.Models.AIRequest;
using System.Collections;

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
}
