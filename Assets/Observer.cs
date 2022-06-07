public interface IObserver 
{
    public abstract void OnNotify(SubjectMessage message);
    public abstract void OnNotify<T>(SubjectMessage message, T parameters) where T : PlayerBeganWarpingEvent;
}