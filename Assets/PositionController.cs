using UnityEngine;

public class PositionController : MonoBehaviour, IObserver
{
    public readonly PositionGrid PositionGrid = new();

    public void Setup(Subject occupiedSpacesSubject)
    {
        occupiedSpacesSubject.AddObserver(this);
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case EnteredPositionEvent enteredPositionEvent:
                PositionGrid.Add(enteredPositionEvent.X,enteredPositionEvent.Y, enteredPositionEvent.GameObject);
                break;
            case LeftPositionEvent leftPositionEvent:
                PositionGrid.Remove(leftPositionEvent.X,leftPositionEvent.Y);
                break;
        }
    }
    
}