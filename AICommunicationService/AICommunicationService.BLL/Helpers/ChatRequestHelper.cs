using AICommunicationService.BLL.Constants;
using AICommunicationService.Common.Models.AIRequest;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace AICommunicationService.BLL.Helpers
{
    /// <summary>
    /// This class assists in building <see cref="ChatRequest"/> objects for various templates with specific temperature settings.
    /// </summary>
    public static class ChatRequestHelper
    {
        private static readonly ChatRequest ChatRequestStable = new() { Temperature = 0, Model = Model.ChatGPTTurbo };
        private static readonly ChatRequest ChatRequestRandom = new() { Temperature = 0.9, Model = Model.ChatGPTTurbo };

        public static ChatRequest GetChatRequestWithOneInputValue(string request, bool isStable, string template)
        {
            if (isStable)
            {
                ChatRequestStable.Messages = new ChatMessage[]
                {
                new ChatMessage(ChatMessageRole.User, string.Format(template, request)),
                };
                return ChatRequestStable;
            }
            ChatRequestRandom.Messages = new ChatMessage[]
                {
                new ChatMessage(ChatMessageRole.User, string.Format(template, request)),
                };
            return ChatRequestRandom;
        }
        public static ChatRequest GetConversationRequest(ConversationRequest conversationRequest)
        {
            ChatRequestRandom.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(AITemplates.ConversationTemplate,
                                                                    conversationRequest.Summary,
                                                                    conversationRequest.HumanAnswer)),
            };
            return ChatRequestRandom;
        }
        public static ChatRequest GetConversationSummaryRequest(ConversationSummaryRequest conversationSummaryRequest)
        {
            ChatRequestRandom.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(AITemplates.ConversationSummaryTemplate,
                                                                    conversationSummaryRequest.CurrentSummary,
                                                                    conversationSummaryRequest.Human,
                                                                    conversationSummaryRequest.AI)),
            };
            return ChatRequestRandom;
        }
    }
}
