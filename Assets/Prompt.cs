public class Prompt
{
    public string Text;
    public object ID;

    public Prompt(object id, string text)
    {
        ID = id;
        Text = text;
    }
}