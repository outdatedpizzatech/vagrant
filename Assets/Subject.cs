using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SubjectMessage
{
    PlayerRequestingWarp,
    PlayerBeganWarping,
    PlayerFinishedWarping,
    ScreenFinishedWipeOut,
    ScreenFinishedWipeIn,
}

public class PlayerBeganWarpingEvent
{
    public PlayerBeganWarpingEvent(Enums.Direction facingDirection, int x, int y)
    {
        FacingDirection = facingDirection;
        X = x;
        Y = y;
    }

    public Enums.Direction FacingDirection { get; }
    public int X { get; }
    public int Y { get; }
}

public class Subject
{
    //A list with observers that are waiting for something to happen
    List<IObserver> observers = new List<IObserver>();

    //Send notifications if something has happened
    public void Notify(SubjectMessage message)
    {
        for (int i = 0; i < observers.Count; i++)
        {
            //Notify all observers even though some may not be interested in what has happened
            //Each observer should check if it is interested in this event
            observers[i].OnNotify(message);
        }
    }

    //Send notifications if something has happened
    public void Notify<T>(SubjectMessage message, T parameters) where T : PlayerBeganWarpingEvent
    {
        for (int i = 0; i < observers.Count; i++)
        {
            //Notify all observers even though some may not be interested in what has happened
            //Each observer should check if it is interested in this event
            observers[i].OnNotify(message, parameters);
        }
    }

    //Add observer to the list
    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    //Remove observer from the list
    public void RemoveObserver(IObserver observer)
    {
    }
}
