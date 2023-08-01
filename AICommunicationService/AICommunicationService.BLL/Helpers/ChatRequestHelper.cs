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
        private static readonly ChatRequest ChatRequestStable = new() { Temperature = 0, Model = Model.GPT4 };
        private static readonly ChatRequest ChatRequestRandom = new() { Temperature = 0.9, Model = Model.GPT4 };

        public static ChatRequest GetDescriptionRequest(string userStories)
        {
            ChatRequestStable.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(AITemplates.DescriptionTemplate, userStories)),
            };
            return ChatRequestStable;
        }
        public static ChatRequest GetDiagramsRequest(string userStories)
        {
            ChatRequestRandom.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(AITemplates.DiagramTemplate, userStories)),
            };
            return ChatRequestRandom;
        }
        public static ChatRequest GetIsRequestClear(string history)
        {
            ChatRequestStable.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(AITemplates.IsRequestClearTemplate, history)),
            };
            return ChatRequestStable;
        }
        public static ChatRequest GetPotentialSummary(string description)
        {
            ChatRequestStable.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(AITemplates.PotentialSummaryTemplate, description)),
            };
            return ChatRequestStable;
        }
        public static ChatRequest GetSummary(string history)
        {
            ChatRequestStable.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(AITemplates.SummaryTemplate, history)),
            };
            return ChatRequestStable;
        }
        public static ChatRequest GetConversation(ConversationRequest conversationRequest)
        {
            ChatRequestRandom.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(AITemplates.ConversationTemplate, conversationRequest.ChatHistory, conversationRequest.HumanValue)),
            };
            return ChatRequestRandom;
        }
    }
}
