using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MessageBoxController : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    public float positionOffset = 4;

    private Subject _flowSubject;
    private TMP_Text _text;
    private List<MessageEnvelope> _messages;
    private float _timeSinceLastLetter;
    private int _shownCharacterIndex;
    private int _messageIndex;
    private PromptController _promptController;

    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;
        _promptController = GetComponent<PromptController>();

        _flowSubject.AddObserver(this);
        _promptController.Setup(flowSubject, this);
    }

    public void Reload(MessageEnvelope message)
    {
        RenderMessage(message);
        _text.alpha = 1;
    }

    public MessageEnvelope CurrentMessage()
    {
        return (_messages[_messageIndex]);
    }

    public bool HasMessages()
    {
        return _messages != null;
    }

    public bool AtEndOfCurrentMessage()
    {
        return (_shownCharacterIndex >= _text.textInfo.characterCount);
    }

    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.AdvanceDialogue:
                if (AtEndOfCurrentMessage())
                {
                    AdvanceMessage();
                }

                break;
        }
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

        if (AtEndOfCurrentMessage())
        {
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

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case InteractionResponseEvent interactionResponseEvent:
                PlayMessages(interactionResponseEvent.Responses);
                break;
        }
    }


    private void PlayMessages(List<MessageEnvelope> messages)
    {
        _messageIndex = 0;
        _messages = messages;
        Show(CurrentMessage());
    }

    private void Show(MessageEnvelope message)
    {
        transform.localScale = Vector3.one;
        _promptController.ResetPrompts(message.Prompts);

        RenderMessage(message);

        _text.alpha = 0;
        _shownCharacterIndex = 0;
    }

    private void RenderMessage(MessageEnvelope message)
    {
        _text.text = message.Message;

        if (message.Prompts.Any())
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
        transform.localScale = Vector3.zero;
    }

    private void AdvanceMessage()
    {
        if (_messageIndex >= _messages.Count - 1)
        {
            if (CurrentMessage().Prompts.Any())
            {
                var selectedPrompt = _promptController.SelectedPrompt();
                _flowSubject.Notify(
                    new PromptResponseEvent(selectedPrompt.ID));
            }
            else
            {
                _flowSubject.Notify(SubjectMessage.EndDialogue);
                Hide();
            }
        }
        else
        {
            _messageIndex++;
            Show(CurrentMessage());
        }
    }
}