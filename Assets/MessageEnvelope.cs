using System.Collections.Generic;

public class MessageEnvelope
{
    public string Message;
    public List<Prompt> Prompts = new List<Prompt>();
}