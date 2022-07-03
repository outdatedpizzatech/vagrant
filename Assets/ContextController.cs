using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ContextController : MonoBehaviour, IObserver
{
    private Subject _flowSubject;

    private readonly List<Enums.ControlContext> _activeContexts = new ();

    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;

        _flowSubject.AddObserver(this);
    }
    
    public void OnNotify(SubjectMessage subjectMessage)
    {
        switch (subjectMessage)
        {
            case SubjectMessage.OpenInteractionMenu:
                AddContext(Enums.ControlContext.InteractionMenu);
                break;
            case SubjectMessage.OpenInventoryMenu:
                AddContext(Enums.ControlContext.InventoryMenu);
                break;
            case SubjectMessage.AdvanceEvent:
                RemoveContext(Enums.ControlContext.Event);
                AddContext(Enums.ControlContext.Event);
                break;
            case SubjectMessage.CloseInventoryMenu:
                RemoveContext(Enums.ControlContext.InventoryMenu);
                break;
            case SubjectMessage.CloseInteractionMenu:
                RemoveContext(Enums.ControlContext.InteractionMenu);
                break;
            case SubjectMessage.EndEventSequence:
                RemoveContext(Enums.ControlContext.Event);
                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case InteractionResponseEvent:
                AddContext(Enums.ControlContext.Event);
                break;
        }
    }

    public bool InHistory(Enums.ControlContext controlContext)
    {
        return (_activeContexts.Any((x) =>
            x == controlContext));
    }

    public Enums.ControlContext Current()
    {
        return _activeContexts.Count == 0
            ? Enums.ControlContext.None
            : _activeContexts.Last();
    }

    private void AddContext(Enums.ControlContext context)
    {
        _activeContexts.Remove(context);
        _activeContexts.Add(context);
    }

    private void RemoveContext(Enums.ControlContext context)
    {
        _activeContexts.Remove(context);
    }
}