using UnityEngine;

public class PositionController : MonoBehaviour, IObserver
{
    public PositionGrid PositionGrid = new PositionGrid();

    public void OnNotify(SubjectMessage subjectMessage)
    {
    }

    public void Setup(Subject occupiedSpacesSubject)
    {
        occupiedSpacesSubject.AddObserver(this);
    }

    public void OnNotify<T>(T parameters)
    {
        if (parameters is EnteredPositionEvent enteredPositionEvent)
        {
            PositionGrid.Add(enteredPositionEvent.X,enteredPositionEvent.Y, enteredPositionEvent.GameObject);
        }
        
        if (parameters is LeftPositionEvent leftPositionEvent)
        {
            PositionGrid.Remove(leftPositionEvent.X,leftPositionEvent.Y);
        }
    }
    
}