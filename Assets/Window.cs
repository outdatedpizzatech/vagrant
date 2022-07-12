using UnityEngine;

public class Window : MonoBehaviour, IObserver
{
    public PersonMovement playerMovement;
    public float positionOffsetX;
    public float positionOffsetY;

    private bool _focused;
    private bool _visible;
    private Subject _contextSubject;

    public void Setup(Subject contextSubject)
    {
        _contextSubject = contextSubject;

        _contextSubject.AddObserver(this);
    }

    public void Show()
    {
        _visible = true;
        GainFocus();

        transform.localScale = Vector3.one;
    }

    public bool IsVisible()
    {
        return _visible;
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

    public void Hide()
    {
        _visible = false;
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

    private void GainFocus(bool broadcast)
    {
        _focused = true;

        BringToFront();

        if (_contextSubject == null || !broadcast) return;
        {
            var windowEvent = new GainFocus(this);
            _contextSubject.Notify(windowEvent);
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
}