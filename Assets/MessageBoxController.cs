using TMPro;
using UnityEngine;

public class MessageBoxController : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    public float positionOffset = 4;

    private InteractionEvent _interactionEvent;
    private Subject _flowSubject;
    private TMP_Text _text;
    private float _timeSinceLastLetter;
    private int _shownCharacterIndex;
    private int _eventStepIndex;
    private EventStep _eventStep;
    private PromptController _promptController;
    private bool _active;

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
            Hide();
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


    public void Reload()
    {
        RenderMessage();
        _text.alpha = 1;
    }

    public bool AtEndOfCurrentMessage()
    {
        return (_shownCharacterIndex >= _text.textInfo.characterCount);
    }

    private void LateUpdate()
    {
        var playerPosition = playerMovement.transform.position;
        var newPosition = new Vector2(playerPosition.x, playerPosition.y - positionOffset);
        transform.position = newPosition;
    }

    private void Update()
    {
        _timeSinceLastLetter += Time.deltaTime;

        if (!_active)
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
    }

    private void Start()
    {
        Hide();
    }

    private void Show()
    {
        _active = true;
        _atEndOfMessage = false;
        transform.localScale = Vector3.one;
        _promptController.ResetPrompts(_interactionEvent.Prompts);

        RenderMessage();

        _text.alpha = 0;
        _shownCharacterIndex = 0;
    }

    private void RenderMessage()
    {
        var eventStep = _interactionEvent.EventSteps[_eventStepIndex];

        if (eventStep.Information is Item item)
        {
            _text.text = $"Received {item.itemName}";
        }
        else if(eventStep.Information is string message)
        {
            _text.text = message;
        }

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

    private void Hide()
    {
        _active = false;
        transform.localScale = Vector3.zero;
    }

    public Prompt SelectedPrompt()
    {
        return _promptController.SelectedPrompt();
    }
}