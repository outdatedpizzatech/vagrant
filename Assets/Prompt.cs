public class Prompt
{
    public string Text;
    private object _id;

    public Prompt(object id, string text)
    {
        _id = id;
        Text = text;
    }
}