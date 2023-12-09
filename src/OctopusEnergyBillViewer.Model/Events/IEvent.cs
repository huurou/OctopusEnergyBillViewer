namespace OctopusEnergyBillViewer.Model.Events
{
    public interface IEvent
    {
        void Publish();

        IDisposable Subscribe(Action handler);

        IDisposable Subscribe(Action handler, Action<Exception> onError);

        IDisposable Subscribe(Func<Task> asyncHandler);

        IDisposable Subscribe(Func<Task> asyncHandler, Action<Exception> onError);
    }

    public interface IEvent<T>
    {
        void Publish(T message);

        IDisposable Subscribe(Action<T> handler);

        IDisposable Subscribe(Action<T> handler, Action<Exception> onError);

        IDisposable Subscribe(Func<T, Task> asyncHandler);

        IDisposable Subscribe(Func<T, Task> asyncHandler, Action<Exception> onError);
    }
}