using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace OctopusEnergyBillViewer.Model.Events
{
    public class Event : IEvent, IDisposable, IObservable<Unit>
    {
        private readonly ISubject<Unit, Unit> subject_ = Subject.Synchronize(new Subject<Unit>());

        public void Publish()
        {
            subject_.OnNext(Unit.Default);
        }

        public IDisposable Subscribe(Action handler)
        {
            return subject_
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Synchronize()
                .Subscribe(_ => Task.Run(handler));
        }

        public IDisposable Subscribe(Action handler, Action<Exception> onError)
        {
            return subject_
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Synchronize()
                .Subscribe(_ => Task.Run(handler), onError);
        }

        public IDisposable Subscribe(Func<Task> asyncHandler)
        {
            return subject_
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Synchronize()
                .Subscribe(_ => asyncHandler());
        }

        public IDisposable Subscribe(Func<Task> asyncHandler, Action<Exception> onError)
        {
            return subject_
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Synchronize()
                .Subscribe(_ => asyncHandler(), onError);
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return subject_.Subscribe(observer);
        }

        public void Dispose()
        {
            if (subject_ is IDisposable d)
            {
                d.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }

    public class Event<T> : IEvent<T>, IDisposable, IObservable<T>
    {
        private readonly ISubject<T, T> subject_ = Subject.Synchronize(new Subject<T>());

        public void Publish(T message)
        {
            subject_.OnNext(message);
        }

        public IDisposable Subscribe(Action<T> handler)
        {
            return subject_
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Synchronize()
                .Subscribe(x => Task.Run(() => handler(x)));
        }

        public IDisposable Subscribe(Action<T> handler, Action<Exception> onError)
        {
            return subject_
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Synchronize()
                .Subscribe(x => Task.Run(() => handler(x)), onError);
        }

        public IDisposable Subscribe(Func<T, Task> asyncHandler)
        {
            return subject_
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Synchronize()
                .Subscribe(x => asyncHandler(x));
        }

        public IDisposable Subscribe(Func<T, Task> asyncHandler, Action<Exception> onError)
        {
            return subject_
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Synchronize()
                .Subscribe(x => asyncHandler(x), onError);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject_.Subscribe(observer);
        }

        public void Dispose()
        {
            if (subject_ is IDisposable d)
            {
                d.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}