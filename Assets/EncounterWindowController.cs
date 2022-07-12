using UnityEngine;

public class EncounterWindowController : MonoBehaviour, IObserver
{
    private Animation _animation;
    private Window _window;
    private Subject _flowSubject;

    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
    }
    
    // Called from animation hook
    public void FinishWipeIn()
    {
        _flowSubject.Notify(SubjectMessage.EncounterFinishedWipeIn);
    }
    
    // Called from animation hook
    public void FinishWipeOut()
    {
        _flowSubject.Notify(SubjectMessage.EndEncounter);
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case SubjectMessage.StartEncounter:
                WipeIn();
                break;
            case SubjectMessage.EncounterStartWipeOut:
                WipeOut();
                break;
            case SubjectMessage.EndEncounter:
                _window.Hide();
                break;
            case SubjectMessage.EncounterFinishedWipeIn:
                _window.Show();
                break;
        }
    }
    
    private void WipeIn()
    {
        _animation.Play("EncounterWipeIn");
    }
    
    private void WipeOut()
    {
        _animation.Play("EncounterWipeOut");
    }
    
    private void Awake()
    {
        _animation = GetComponent<Animation>();
        _window = GetComponent<Window>();
    }
}
