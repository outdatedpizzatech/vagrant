using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitBoxController : MonoBehaviour, IObserver
{
    private Subject _flowSubject;
    private Window _window;
    private Image _image;
    private InteractionEvent _interactionEvent;
    private int _eventStepIndex;
    
    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;

        _flowSubject.AddObserver(this);
    }
    
    private void Show()
    {
        var eventStep = _interactionEvent.EventSteps[_eventStepIndex];

        if (eventStep.Information is Message message)
        {
            if (message.SpeakingAnimation != null)
            {
                _image.GetComponent<Animator>().Play(message.SpeakingAnimation.name);
                _window.Show();
                return;
            }
        }
        
        _window.Hide();
    }
    private void Awake()
    {
        _image = transform.Find("Image").GetComponent<Image>();
        _window = GetComponent<Window>();
    }

    public void OnNotify(SubjectMessage message)
    {
        if (message == SubjectMessage.EndEventSequence)
        {
            _window.Hide();
        }

        if (message == SubjectMessage.ReachedEndOfMessage)
        {
            var eventStep = _interactionEvent.EventSteps[_eventStepIndex];

            if (eventStep.Information is Message messageFromInteraction)
            {
                if (messageFromInteraction.IdleAnimation != null)
                {
                    _image.GetComponent<Animator>().Play(messageFromInteraction.IdleAnimation.name);
                }
            }
        }
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case InteractionResponseEvent interactionResponseEvent:
                _interactionEvent = interactionResponseEvent.InteractionEvent;
                break;
            case StartEventStep startEventStepEvent:
                _eventStepIndex = startEventStepEvent.EventStepIndex;
                Show();
                break;
        }
    }
}
