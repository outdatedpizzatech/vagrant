using System;
using UnityEngine;

public class WiperController : MonoBehaviour, IObserver
{
    private Animation _animation;
    private Subject _subject;
    
    public void Setup(Subject subject)
    {
        _subject = subject;
        subject.AddObserver(this);
    }

    // animation hook
    public void FinishWipeOut()
    {
        _subject.Notify(SubjectMessage.ScreenFinishedWipeOut);
        WipeIn();
    }

    // animation hook
    public void FinishWipeIn()
    {
        _subject.Notify(SubjectMessage.ScreenFinishedWipeIn);
    }
    
    public void OnNotify<T>(T parameters)
    {
        if (parameters is PlayerBeganWarpingEvent)
        {
            WipeOut();
        }
    }
    
    private void WipeOut()
    {
        _animation.Play("WipeOut");
    }
    
    private void WipeIn()
    {
        _animation.Play("WipeIn");
    }

    private void Update()
    {
        if (_subject == null)
        {
            throw new Exception("subject not supplied");
        }
    }
    
    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }
}
