using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;

namespace OctopusEnergyBillViewer.Presentation.WPF;

public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected CompositeDisposable Disposables { get; } = [];

    protected void SetProperty<T>(ref T target, T value, [CallerMemberName] string caller = "")
    {
        target = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
    }

    public void Dispose()
    {
        Disposables.Dispose();
        GC.SuppressFinalize(this);
    }
}