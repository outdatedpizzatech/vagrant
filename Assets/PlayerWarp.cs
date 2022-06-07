using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerWarp : MonoBehaviour
{
    [CanBeNull] private Subject _subject;

    public void Setup(Subject subject)
    {
        _subject = subject;
    }

    public void WarpTo(int x, int y, Enums.Direction direction)
    {
        _subject.Notify(SubjectMessage.PlayerRequestingWarp);
        StartCoroutine(BeginWarping(0.25f, x, y, direction));
    }
    
    private IEnumerator BeginWarping(float waitTime, int x, int y, Enums.Direction direction)
    {
        yield return new WaitForSeconds(waitTime);
        
        /*
         * TODO: this is pretty sketchy. We're deferring the position setting because it
         * causes a bug if it's done immediately. This is a race condition waiting to happen.
         */ 
        
        var parameters = new PlayerBeganWarpingEvent(direction, x, y);
        
        _subject.Notify(parameters);
    }
}
