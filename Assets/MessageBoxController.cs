using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxController : MonoBehaviour, IObserver
{
    private Subject _flowSubject;
    private Text _text;
    
    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
    }
    
    private void Start()
    {
        Hide();
        _text = transform.Find("Text").GetComponent<Text>();
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
    }

    private void Hide()
    {
        transform.localScale = Vector3.zero;
    }
}
