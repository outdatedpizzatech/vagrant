using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    public float positionOffsetX;
    public float positionOffsetY;

    private bool _focused;
    private Subject _contextSubject;

    public void Setup(Subject contextSubject)
    {
        _contextSubject = contextSubject;

        _contextSubject.AddObserver(this);
    }

    public void Show()
    {
        GainFocus();

        transform.localScale = Vector3.one;
    }

    public void OnNotify(SubjectMessage subjectMessage)
    {
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case RegainFocus windowEvent:
                if (this == windowEvent.Window)
                {
                    GainFocus(false);
                }

                break;
        }
    }

    private void LateUpdate()
    {
        var playerPosition = playerMovement.transform.position;
        var newPosition = new Vector2(playerPosition.x + positionOffsetX, playerPosition.y + positionOffsetY);
        transform.position = newPosition;
    }

    private void Start()
    {
        Hide();
    }

    private void BringToFront()
    {
        var currentParent = transform.parent;
        var emptyParent = GameObject.Find("Empty").transform;
        transform.SetParent(emptyParent, true);
        transform.SetParent(currentParent, true);
    }

    public void Hide()
    {
        LoseFocus();

        transform.localScale = Vector3.zero;

        var windowEvent = new DismissWindow(this);

        if (_contextSubject != null)
        {
            _contextSubject.Notify(windowEvent);
        }
    }

    public bool IsFocused()
    {
        return _focused;
    }

    public void LoseFocus()
    {
        _focused = false;
    }

    public void GainFocus()
    {
        GainFocus(true);
    }

    public void GainFocus(bool broadcast)
    {
        _focused = true;

        BringToFront();

        if (_contextSubject != null)
        {
            if (broadcast)
            {
                var windowEvent = new GainFocus(this);
                _contextSubject.Notify(windowEvent);
            }
        }
    }
}