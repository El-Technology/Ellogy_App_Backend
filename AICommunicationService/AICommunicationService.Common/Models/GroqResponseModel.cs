namespace AICommunicationService.Common.Models;

public class Choice
{
    public int index { get; set; }
    public Message message { get; set; }
    public object logprobs { get; set; }
    public string finish_reason { get; set; }
}

public class Message
{
    public string role { get; set; }
    public string content { get; set; }
}

public class GroqResponseModel
{
    public string id { get; set; }
    public string @object { get; set; }
    public int created { get; set; }
    public string model { get; set; }
    public List<Choice> choices { get; set; }
    public Usage usage { get; set; }
    public string system_fingerprint { get; set; }
    public XGroq x_groq { get; set; }
}

public class Usage
{
    public int prompt_tokens { get; set; }
    public double prompt_time { get; set; }
    public int completion_tokens { get; set; }
    public double completion_time { get; set; }
    public int total_tokens { get; set; }
    public double total_time { get; set; }
}

public class XGroq
{
    public string id { get; set; }
}