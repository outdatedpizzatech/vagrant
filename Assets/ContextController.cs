using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ContextController : MonoBehaviour, IObserver
{
    private Subject _flowSubject;

    public List<Enums.ControlContext> activeContexts = new ();

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

    private void AddContext(Enums.ControlContext context)
    {
        activeContexts.Remove(context);
        activeContexts.Add(context);
    }

    private void RemoveContext(Enums.ControlContext context)
    {
        activeContexts.Remove(context);
    }
}
