public interface IObserver 
{
    public abstract void OnNotify<T>(T parameters);
}