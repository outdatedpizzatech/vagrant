public class Prompt
{
    public readonly string Text;
    public readonly object ID;

    public Prompt(object id, string text)
    {
        ID = id;
        Text = text;
    }
}