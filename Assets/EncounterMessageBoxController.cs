using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EncounterMessageBoxController : MonoBehaviour, IObserver
{
    private InteractionEvent _interactionEvent;
    private Subject _encounterSubject;
    private TMP_Text _text;
    private float _timeSinceLastLetter;
    private int _shownCharacterIndex;
    private int _eventStepIndex;
    private EventStep _eventStep;
    private Window _window;

    private bool _atEndOfMessage;

    public InteractionEvent InteractionEvent()
    {
        return _interactionEvent;
    }

    public void Setup(Subject encounterSubject)
    {
        _encounterSubject = encounterSubject;

        _encounterSubject.AddObserver(this);
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
            _encounterSubject.Notify(SubjectMessage.ReachedEndOfMessage);

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
