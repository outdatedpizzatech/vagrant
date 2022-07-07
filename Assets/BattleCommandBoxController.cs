using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleCommandBoxController : MonoBehaviour, IObserver
{
    private Window _window;
    private Subject _flowSubject;
    private TMP_Text _text;

    private void Show()
    {
        _window.Show();
        RenderText();
    }

    private void Awake()
    {
        _window = GetComponent<Window>();
        _text = transform.Find("Text").GetComponent<TMP_Text>();
    }

    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;

        _flowSubject.AddObserver(this);
    }

    public void OnNotify(SubjectMessage message)
    {
        if (message == SubjectMessage.StartEncounter)
        {
            Show();
        }

        if (message == SubjectMessage.EndEncounter)
        {
            _window.Hide();
        }

        if (message == SubjectMessage.MenuSelection && _window.IsFocused())
        {
            _flowSubject.Notify(SubjectMessage.EndEncounter);
        }
    }

    public void OnNotify<T>(T parameters)
    {
    }

    private void RenderText()
    {
        _text.text = "\n<sprite anim='0,1,4'> RUN";
    }
}