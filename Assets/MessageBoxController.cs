using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class MessageBoxController : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    
    private Subject _flowSubject;
    private TMP_Text _text;
    private List<MessageEnvelope> _messages;
    private float _timeSinceLastLetter = 0;
    private float _timeBetweenLetters = 0.025f;
    private int _shownCharacterIndex = 0;
    private int _messageIndex = 0;
    private int _selectedPromptIndex = 0;
    private float _timeSinceLastPromptChange = 0;
    private float _timeBetweenPromptChanges = 0.25f;
    
    private bool AtEndOfCurrentMessage()
    {
        return (_shownCharacterIndex >= _text.textInfo.characterCount);
    }

    void LateUpdate()
    {
        var newPosition = new Vector2(playerMovement.transform.position.x, playerMovement.transform.position.y - 4);
        transform.position = newPosition;
    }

    private void Update()
    {
        _timeSinceLastPromptChange += Time.deltaTime;
        
        if (AtEndOfCurrentMessage())
        {
            return;
        }
        
        ShowCharactersUpTo(_shownCharacterIndex);

        _timeSinceLastLetter += Time.deltaTime;

        if (_timeSinceLastLetter >= _timeBetweenLetters)
        {
            _shownCharacterIndex++;
            _timeSinceLastLetter = 0;
        }
    }
    

    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
    }
    
    private void Start()
    {
        Hide();
        _text = transform.Find("Text").GetComponent<TMP_Text>();
    }

    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.AdvanceDialogue:
                if (AtEndOfCurrentMessage())
                {
                    if (_messageIndex >= _messages.Count - 1)
                    {
                        _flowSubject.Notify(SubjectMessage.EndDialogue);
                        Hide();
                    }
                    else
                    {
                        _messageIndex++;
                        Show(_messages[_messageIndex]);
                    }
                }
                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        if (parameters is InteractionResponseEvent interactionResponseEvent)
        {
            PlayMessages(interactionResponseEvent.Responses);
        }
        if (parameters is MenuNavigation menuNavigation)
        {
            if (_messages != null && _timeSinceLastPromptChange > _timeBetweenPromptChanges)
            {
                _timeSinceLastPromptChange = 0;
                var currentMessage = _messages[_messageIndex];
                var promptCount = currentMessage.Prompts.Count();
                if (promptCount > 0 && AtEndOfCurrentMessage())
                {
                    if (menuNavigation.Direction == Enums.Direction.Down)
                    {
                        _selectedPromptIndex++;
                    } else if (menuNavigation.Direction == Enums.Direction.Up)
                    {
                        _selectedPromptIndex--;
                    }

                    _selectedPromptIndex = Mathf.Abs(_selectedPromptIndex % promptCount);
                    
                    Reload(_messages[_messageIndex]);
                }
            }
        }
    }

    private void PlayMessages(List<MessageEnvelope> messages)
    {
        _messageIndex = 0;
        _messages = messages;
        Show(messages[_messageIndex]);
    }

    private void Show(MessageEnvelope message)
    {
        transform.localScale = Vector3.one;
        _selectedPromptIndex = 0;
        
        RenderMessage(message);
        
        _text.alpha = 0;
        _shownCharacterIndex = 0;
    }
    private void Reload(MessageEnvelope message)
    {
        RenderMessage(message);
        _text.alpha = 1;
    }

    private void RenderMessage(MessageEnvelope message)
    {
        _text.text = message.Message;
        
        var promptIndex = 0;

        if (message.Prompts.Any())
        {
            _text.text += "\n";
            foreach (var prompt in message.Prompts)
            {
                if (promptIndex == _selectedPromptIndex)
                {
                    _text.text += $"\n> {prompt.Text}";
                }
                else
                {
                    _text.text += $"\n  {prompt.Text}";
                }
                promptIndex++;
            }
        }
    }

    private void ShowCharactersUpTo(int characterIndex)
    {
        
        _text.ForceMeshUpdate();

        for (int i = 0; i <= characterIndex; i++)
        {
            var info = _text.textInfo.characterInfo[i];
            int meshIndex = info.materialReferenceIndex;
            int vertexIndex = info.vertexIndex;
            
            Color32[] vertexColors = _text.textInfo.meshInfo[meshIndex].colors32;
            vertexColors[vertexIndex + 0].a = (byte)255;
            vertexColors[vertexIndex + 1].a = (byte)255;
            vertexColors[vertexIndex + 2].a = (byte)255;
            vertexColors[vertexIndex + 3].a = (byte)255;
        }
        
        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32); 
    }

    private void Hide()
    {
        transform.localScale = Vector3.zero;
    }
}
