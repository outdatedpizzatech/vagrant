using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxController : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    
    private Subject _flowSubject;
    private TMP_Text _text;

    private float timeSinceLastLetter = 0;
    private float timeBetweenLetters = 0.025f;
    private int shownCharacterIndex = 0;

    void LateUpdate()
    {
        var newPosition = new Vector2(playerMovement.transform.position.x, playerMovement.transform.position.y - 4);
        transform.position = newPosition;
    }

    private void Update()
    {
        if (shownCharacterIndex >= _text.textInfo.characterCount)
        {
            return;
        }
        
        ShowCharactersUpTo(shownCharacterIndex);

        timeSinceLastLetter += Time.deltaTime;

        if (timeSinceLastLetter >= timeBetweenLetters)
        {
            shownCharacterIndex++;
            timeSinceLastLetter = 0;
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
            case SubjectMessage.EndDialogue:
                Hide();
                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        if (parameters is InteractionResponseEvent interactionResponseEvent)
        {
            Show(interactionResponseEvent.Message);
        }
    }

    private void Show(string message)
    {
        transform.localScale = Vector3.one;
        _text.text = message;
        _text.alpha = 0;
        shownCharacterIndex = 0;
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
