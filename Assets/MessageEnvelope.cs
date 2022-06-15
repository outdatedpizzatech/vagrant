using System.Collections.Generic;

public class MessageEnvelope
{
    public string Message;
    public readonly List<Prompt> Prompts = new();
    public string Item;
    
    /*
     * While this seems like a MessageEnvelope could receive any of these items, there are business rules which
     * invalidate certain members. For example, a prompt is only used for the last MessageEnvelope in a list
     *
     * This likely means that the domain hierarchy of MessageEnvelope is wrong and should change
     */
}