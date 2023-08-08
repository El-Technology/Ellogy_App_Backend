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
        public static ChatRequest GetConversationRequest(ConversationRequest conversationRequest, string template)
        {
            ChatRequestRandom.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(template,
                                                                    conversationRequest.Summary,
                                                                    conversationRequest.HumanAnswer))
            };
            return ChatRequestRandom;
        }
        public static ChatRequest GetConversationSummaryRequest(ConversationSummaryRequest conversationSummaryRequest, string template)
        {
            ChatRequestRandom.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(template,
                                                                    conversationSummaryRequest.CurrentSummary,
                                                                    conversationSummaryRequest.Human,
                                                                    conversationSummaryRequest.AI))
            };
            return ChatRequestRandom;
        }

        public static ChatRequest GetDiagramRequest(DiagramRequest diagramRequest, string template)
        {
            ChatRequestStable.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(template,
                                                                    diagramRequest.UserStories,
                                                                    diagramRequest.Usecase))
            };
            return ChatRequestStable;
        }

        public static ChatRequest GetUsecaseConversationRequest(UsecaseConversationRequest usecaseConversationRequest, string template)
        {
            ChatRequestRandom.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(template,
                                                                    usecaseConversationRequest.ChatHistory,
                                                                    usecaseConversationRequest.UserValue))
            };
            return ChatRequestRandom;
        }

        public static ChatRequest GetNotificationConversationRequest(NotificationConversationRequest notificationConversationRequest, string template)
        {
            ChatRequestRandom.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(template,
                                                                    notificationConversationRequest.ChatHistory,
                                                                    notificationConversationRequest.UserValue))
            };
            return ChatRequestRandom;
        }

        public static ChatRequest GetDiagramCorrectionRequest(DiagramCorrectionRequest diagramCorrection, string template)
        {
            ChatRequestStable.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(template,
                                                                   diagramCorrection.Diagram,
                                                                   diagramCorrection.Requirements))
            };
            return ChatRequestStable;
        }

        public static ChatRequest GetDescriptionTableRequest(DescriptionTableRequest descriptionTableRequest, string template)
        {
            ChatRequestStable.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(template,
                                                                   descriptionTableRequest.Description,
                                                                   descriptionTableRequest.Usecase))
            };
            return ChatRequestStable;
        }
    }
}
