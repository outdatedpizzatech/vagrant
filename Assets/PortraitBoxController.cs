using UnityEngine;
using UnityEngine.UI;

public class PortraitBoxController : MonoBehaviour, IObserver
{
    private Subject _flowSubject;
    private Window _window;
    private Image _image;
    private InteractionEvent _interactionEvent;
    private int _eventStepIndex;
    private Animator _animator;

    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case FlowTopic.LoseInteractionTarget:
                _window.Hide();
                break;
            case EventTopic.ReachedEndOfMessage:
                var eventStep = _interactionEvent.EventSteps[_eventStepIndex];

                if (eventStep.Information is not Message messageFromInteraction) return;
                if (messageFromInteraction.IdleAnimation != null)
                {
                    _animator.Play(messageFromInteraction.IdleAnimation.name);
                }

                break;
            case InteractionResponseEvent interactionResponseEvent:
                _interactionEvent = interactionResponseEvent.InteractionEvent;
                break;
            case StartEventStep startEventStepEvent:
                _eventStepIndex = startEventStepEvent.EventStepIndex;
                Show();
                break;
        }
    }

    private void Show()
    {
        var eventStep = _interactionEvent.EventSteps[_eventStepIndex];

        if (eventStep.Information is Message message)
        {
            if (message.SpeakingAnimation != null)
            {
                _animator.Play(message.SpeakingAnimation.name);
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
        _animator = _image.GetComponent<Animator>();
    }
}