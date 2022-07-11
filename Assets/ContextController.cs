using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContextController : MonoBehaviour, IObserver
{
    private Subject _contextSubject;

    private readonly List<Window> _activeContexts = new ();

    public void Setup(Subject contextSubject)
    {
        _contextSubject = contextSubject;

        _contextSubject.AddObserver(this);
    }

    private void Update()
    {
        print(_activeContexts.Count);
    }

    public void OnNotify(SubjectMessage subjectMessage)
    {
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case GainFocus windowEvent:
                AddContext(windowEvent.Window);
                break;
            case DismissWindow windowEvent:
                RemoveContext(windowEvent.Window);
                break;
        }
    }

    private void AddContext(Window context)
    {
        _activeContexts.Remove(context);
        _activeContexts.Add(context);
    }

    private void RemoveContext(Window context)
    {
        _activeContexts.Remove(context);

        if (_activeContexts.Count > 0)
        {
            var windowEvent = new RegainFocus(_activeContexts.Last());
            _contextSubject.Notify(windowEvent);
        }
    }

    public bool Any()
    {
        return _activeContexts.Count > 0;
    }
}