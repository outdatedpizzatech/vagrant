public interface IObserver 
{
    public abstract void OnNotify(SubjectMessage message);
    public abstract void OnNotify<T>(T parameters);
}