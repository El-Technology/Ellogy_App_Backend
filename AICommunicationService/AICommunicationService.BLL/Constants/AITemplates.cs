namespace AICommunicationService.BLL.Constants
{
    /// <summary>
    /// This class contains templates and methods for communication with Chat GPT (Generative Pre-trained Transformer).
    /// It provides functionalities to send various types of requests to Chat GPT and receive responses from the model.
    /// </summary>
    public static class AITemplates
    {
        public const string DescriptionTemplate =
           "User stories:" +
           "{0}";
        public const string DiagramTemplate =
            "User stories:" +
            "{0}";
        public const string IsRequestClearTemplate =
            "Current conversation:" +
            "{0}";
        public const string PotentialSummaryTemplate =
            "Project description:" +
            "{0}";
        public const string SummaryTemplate =
            "Current conversation:" +
            "{0}";
        public const string ConversationTemplate =
            "Current conversation:" +
            "{0}" +
            "Human: {1}" +
            "AI:";
        public const string ConversationSummaryTemplate =
            "Current summary:" +
            "{0}" +
            "New lines of conversation:" +
            "Human: {1}" +
            "AI: {2}" +
            "New summary:";
    }
}
