namespace AICommunicationService.BLL.Constants
{
    /// <summary>
    /// This class contains templates and methods for communication with Chat GPT (Generative Pre-trained Transformer).
    /// It provides functionalities to send various types of requests to Chat GPT and receive responses from the model.
    /// </summary>
    public static class AITemplates
    {
        public const string DescriptionTemplate = "Generate project description and short title based on user stories. Description length has to be around 50 words. Send response in JSON format with description and title fields." +
           "User stories:" +
           "{0}";
        public const string DiagramTemplate = "Welcome! I am an AI capable of generating UML diagrams based on the context of user stories. Please describe the functionality or software requirement you want to visualize with a UML diagram, and I'll create the most relevant UML representation for it. Include one diagram from each of these categories: use case diagram, sequence diagram, flowchart. UML diagrams should be generated as PlantUML code. Return response in JSON format with array of objects with type and code fields." +
            "User stories:" +
            "{0}";
        public const string IsRequestClearTemplate = "Analyze current conversation and determine whether user has provided basic information about system. Return boolean value." +
            "Current conversation:" +
            "{0}";
        public const string PotentialSummaryTemplate = "As the AI project manager, you need to analyze the description of project and generate a list of possible requirements for the successful development and implementation of the project. Consider the technical aspects, user experience, scalability, security, and any other relevant factors to ensure the project's success. Format text of requirement this way: \"As a role, I want to requirement, so benefit\". Send response in JSON format, JSON object should be an array of objects with requirement, role and benefit fields. Send 20 requirements. Give 4 requirements from each role." +
            "Project description:" +
            "{0}";
        public const string SummaryTemplate = "Summarize the current conversation only in distinguished user stories each starting with \"As a role, I want: \" where role is one of following: user, ai. Transfer each user requirement into a separate user story. Ignore lines that are decline or approve without additional clarifications. If there are multiple requirements in story, split them in separate stories. Send response only in JSON format it should always be an array of objects with story, role and importance fields." +
            "Current conversation:" +
            "{0}";
        public const string ConversationTemplate = "You are an IT Requirements engineer. Your role is to ask questions to humans who are requesting IT services from the IT department. After that, the system will ask one question and wait for the human to answer. Upon receiving the answer, the system will move to new category, of the questions, prioritizing categories that has not been discussed in current conversation yet, and repeat the previous process. The categories of questions to ask during the conversation are Functional requirements, Performance, Scalability, Portability and compatibility, Reliability, Maintainability, Availability, Security, Usability. Try to avoid asking similar questions too often." +
            "Current conversation:" +
            "{0}" +
            "Human: {1}" +
            "AI:";
        public const string ConversationSummaryTemplate = "Progressively summarize the lines of conversation provided, adding onto the previous summary returning a new summary." +
            "EXAMPLE" +
            "Current summary:" +
            "The human asks what the AI thinks of artificial intelligence. The AI thinks artificial intelligence is a force for good." +
            "New lines of conversation:" +
            "Human: Why do you think artificial intelligence is a force for good?" +
            "AI: Because artificial intelligence will help humans reach their full potential." +
            "New summary:" +
            "The human asks what the AI thinks of artificial intelligence. The AI thinks artificial intelligence is a force for good because it will help humans reach their full potential." +
            "END OF EXAMPLE" +
            "Current summary:" +
            "{0}" +
            "New lines of conversation:" +
            "Human: {1}" +
            "AI: {2}" +
            "New summary:";
    }
}
