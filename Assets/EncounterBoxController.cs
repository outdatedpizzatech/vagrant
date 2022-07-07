using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterBoxController : MonoBehaviour, IObserver
{
    private Animation _animation;
    private Window _window;
    private Subject _flowSubject;
    
    private void Awake()
    {
        _animation = GetComponent<Animation>();
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
            WipeIn();
        }
        if (message == SubjectMessage.EndEncounter)
        {
            _window.Hide();
        }
        if (message == SubjectMessage.EncounterFinishedWipeIn)
        {
            _window.Show();
        }
    }
    
    public void FinishWipeIn()
    {
        _flowSubject.Notify(SubjectMessage.EncounterFinishedWipeIn);
    }

    public void OnNotify<T>(T parameters)
    {
    }
    
    private void WipeIn()
    {
        _animation.Play("EncounterWipeIn");
    }
}
