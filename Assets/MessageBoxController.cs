using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxController : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    
    private Subject _flowSubject;
    private TMP_Text _text;
    private List<string> _messages;
    private float _timeSinceLastLetter = 0;
    private float _timeBetweenLetters = 0.025f;
    private int _shownCharacterIndex = 0;
    private int _messageIndex = 0;

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
            PlayMessages(interactionResponseEvent.Responses.Select((r) => r.Message).ToList());
        }
    }

    private void PlayMessages(List<string> messages)
    {
        _messageIndex = 0;
        _messages = messages;
        Show(messages[_messageIndex]);
    }

    private void Show(string message)
    {
        transform.localScale = Vector3.one;
        _text.text = message;
        _text.alpha = 0;
        _shownCharacterIndex = 0;
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
