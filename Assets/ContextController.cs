using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextController : MonoBehaviour, IObserver
{
    private Subject _flowSubject;
    
    public enum ControlContext
    {
        Event,
        FollowUpMenu,
        InventoryMenu,
        PromptMenu,
        None
    }
    
    public List<ControlContext> activeContexts = new ();

    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;

        _flowSubject.AddObserver(this);
    }
    
    public void OnNotify(SubjectMessage subjectMessage)
    {
        switch (subjectMessage)
        {
            case SubjectMessage.RequestFollowUpEvent:
                AddContext(ControlContext.FollowUpMenu);
                break;
            case SubjectMessage.OpenMenuEvent:
                AddContext(ControlContext.InventoryMenu);
                break;
            case SubjectMessage.AdvanceEvent:
                RemoveContext(ControlContext.Event);
                AddContext(ControlContext.Event);
                break;
            case SubjectMessage.CloseMenuEvent:
                RemoveContext(ControlContext.InventoryMenu);
                break;
            case SubjectMessage.EndFollowUpEvent:
                RemoveContext(ControlContext.FollowUpMenu);
                break;
            case SubjectMessage.EndEventSequenceEvent:
                RemoveContext(ControlContext.Event);
                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case InteractionResponseEvent:
                AddContext(ControlContext.Event);
                break;
        }
    }

    private void AddContext(ControlContext context)
    {
        activeContexts.Remove(context);
        activeContexts.Add(context);
    }

    private void RemoveContext(ControlContext context)
    {
        activeContexts.Remove(context);
    }
}
