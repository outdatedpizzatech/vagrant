using System.Collections.Generic;

public class Subject
{
    private readonly List<IObserver> _observers = new();

    public Subject(string topic)
    {
    }

    public void Notify(SubjectMessage message)
    {
        foreach (var t in _observers)
        {
            t.OnNotify(message);
        }
    }

    public void Notify<T>(T parameters)
    {
        foreach (var t in _observers)
        {
            t.OnNotify(parameters);
        }
    }

    public void AddObserver(IObserver observer)
    {
        _observers.Add(observer);
    }
}
