using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBoxController : MonoBehaviour, IObserver
{
    private InteractionEvent _interactionEvent;
    private Subject _flowSubject;
    private TMP_Text _text;
    private float _timeSinceLastLetter;
    private int _shownCharacterIndex;
    private int _eventStepIndex;
    private EventStep _eventStep;
    private PromptController _promptController;
    private Window _window;

    private bool _atEndOfMessage;

    public InteractionEvent InteractionEvent()
    {
        return _interactionEvent;
    }

    public void Setup(Subject flowSubject)
    {
        _promptController = GetComponent<PromptController>();
        _promptController.Setup(flowSubject, this);
        _flowSubject = flowSubject;

        _flowSubject.AddObserver(this);
    }

    public void OnNotify(SubjectMessage message)
    {
        if (message == SubjectMessage.EndEventSequence)
        {
            _window.Hide();
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


    public void RenderText()
    {
        RenderMessage();
        _text.alpha = 1;
    }

    public bool AtEndOfCurrentMessage()
    {
        return (_shownCharacterIndex >= _text.textInfo.characterCount);
    }

    public Prompt SelectedPrompt()
    {
        return _promptController.SelectedPrompt();
    }

    public bool IsFocused()
    {
        return _window.IsFocused();
    }

    private void Update()
    {
        _timeSinceLastLetter += Time.deltaTime;

        if (!IsFocused())
        {
            return;
        }

        if (AtEndOfCurrentMessage())
        {
            if (_atEndOfMessage) return;
            _atEndOfMessage = true;
            _flowSubject.Notify(SubjectMessage.ReachedEndOfMessage);

            return;
        }
        
        ShowCharactersUpTo(_shownCharacterIndex);

        Utilities.Debounce(ref _timeSinceLastLetter, 0.025f, () => { _shownCharacterIndex++; });
    }

    private void Awake()
    {
        _text = transform.Find("Text").GetComponent<TMP_Text>();
        _window = GetComponent<Window>();
    }

    private void Show()
    {
        _window.Show();
        _atEndOfMessage = false;

        if (_interactionEvent.Information is List<Prompt> prompts)
        {
            _promptController.ResetPrompts(prompts);
        }else {
            _promptController.ResetPrompts(new List<Prompt>());
        }

        RenderMessage();

        _text.alpha = 0;
        _shownCharacterIndex = 0;
    }

    private void RenderMessage()
    {
        var eventStep = _interactionEvent.EventSteps[_eventStepIndex];

        _text.text = eventStep.Information switch
        {
            Item item => $"Received {item.itemName}",
            Message message => message.Content,
            _ => _text.text
        };

        if (_eventStepIndex == _interactionEvent.EventSteps.Count - 1)
        {
            _text.text += _promptController.PromptContent();
        }
    }

    private void ShowCharactersUpTo(int characterIndex)
    {
        _text.ForceMeshUpdate();

        for (var i = 0; i <= characterIndex; i++)
        {
            var info = _text.textInfo.characterInfo[i];
            var meshIndex = info.materialReferenceIndex;
            var vertexIndex = info.vertexIndex;

            var vertexColors = _text.textInfo.meshInfo[meshIndex].colors32;
            vertexColors[vertexIndex + 0].a = 255;
            vertexColors[vertexIndex + 1].a = 255;
            vertexColors[vertexIndex + 2].a = 255;
            vertexColors[vertexIndex + 3].a = 255;
        }

        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}