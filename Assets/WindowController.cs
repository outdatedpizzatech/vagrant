using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WindowController : MonoBehaviour, IObserver
{
    private Subject _subject;
    private readonly List<Window> _activeWindows = new ();

    public void Setup(Subject subject)
    {
        _subject = subject;
        _subject.AddObserver(this);
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case GainFocus windowEvent:
                AddWindow(windowEvent.Window);
                break;
            case DismissWindow windowEvent:
                RemoveWindow(windowEvent.Window);
                break;
        }
    }

    public bool Any()
    {
        return _activeWindows.Any();
    }

    private void AddWindow(Window context)
    {
        _activeWindows.Remove(context);
        _activeWindows.Add(context);
    }

    private void RemoveWindow(Window context)
    {
        _activeWindows.Remove(context);

        if (!_activeWindows.Any()) return;
        var windowEvent = new RegainFocus(_activeWindows.Last());
        _subject.Notify(windowEvent);
    }
}