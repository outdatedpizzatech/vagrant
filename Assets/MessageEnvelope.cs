using System.Collections.Generic;

public class MessageEnvelope
{
    public string Message;
    public readonly List<Prompt> Prompts = new();
    public string Item;
}