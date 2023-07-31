using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace AICommunicationService.BLL.Helpers
{
    public static class ChatRequestHelper
    {
        private const string DescriptionTemplate = "Generate project description and short title based on user stories. Description length has to be around 50 words. Send response in JSON format with description and title fields." +
            "User stories:" +
            "{0}";
        private const string DiagramTemplate = "Welcome! I am an AI capable of generating UML diagrams based on the context of user stories. Please describe the functionality or software requirement you want to visualize with a UML diagram, and I'll create the most relevant UML representation for it. Include one diagram from each of these categories: use case diagram, sequence diagram, flowchart. UML diagrams should be generated as PlantUML code. Return response in JSON format with array of objects with type and code fields." +
            "User stories:" +
            "{0}";
        private const string IsRequestClear = "Analyze current conversation and determine whether user has provided basic information about system. Return boolean value." +
            "Current conversation:" +
            "{0}";
        private const string PotentialSummary = "As the AI project manager, you need to analyze the description of project and generate a list of possible requirements for the successful development and implementation of the project. Consider the technical aspects, user experience, scalability, security, and any other relevant factors to ensure the project's success. Format text of requirement this way: \"As a role, I want to requirement, so benefit\". Send response in JSON format, JSON object should be an array of objects with requirement, role and benefit fields. Send 20 requirements. Give 4 requirements from each role." +
            "Project description:" +
            "{0}";

        private static readonly ChatRequest ChatRequestStable = new() { Temperature = 0, Model = Model.GPT4 };
        private static readonly ChatRequest ChatRequestRandom = new() { Temperature = 0.9, Model = Model.GPT4 };

        public static ChatRequest GetDescriptionRequest(string userStories)
        {
            ChatRequestStable.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(DescriptionTemplate, userStories)),
            };
            return ChatRequestStable;
        }
        public static ChatRequest GetDiagramsRequest(string userStories)
        {
            ChatRequestRandom.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(DiagramTemplate, userStories)),
            };
            return ChatRequestRandom;
        }
        public static ChatRequest GetIsRequestClear(string history)
        {
            ChatRequestStable.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(IsRequestClear, history)),
            };
            return ChatRequestStable;
        }
        public static ChatRequest GetPotentialSummary(string description)
        {
            ChatRequestStable.Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.User, string.Format(PotentialSummary, description)),
            };
            return ChatRequestStable;
        }
    }
}
