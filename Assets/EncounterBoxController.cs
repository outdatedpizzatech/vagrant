using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterBoxController : MonoBehaviour, IObserver
{
    private Window _window;
    private Subject _flowSubject;
    
    private void Awake()
    {
        _window = GetComponent<Window>();
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
            _window.Show();
        }
    }

    public void OnNotify<T>(T parameters)
    {
    }
}
