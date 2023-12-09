using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace OctopusEnergyBillViewer.Presentation.WPF.Dialogs;

public static class DialogService
{
    public static void Show<TDialog>(Action? onOpened = null, Action<bool?>? onClosed = null)
        where TDialog : Window, new()
    {
        Application.Current.Dispatcher.Invoke(
            () =>
            {
                if (ShowInner<TDialog>(out var dialog))
                {
                    var result = dialog.ShowDialog();
                    onOpened?.Invoke();
                    onClosed?.Invoke(result);
                }
            });
    }

    public static void Show<TDialog, TDialogVM>(Action<TDialogVM>? onOpened = null, Action<bool?, TDialogVM>? onClosed = null)
        where TDialog : Window, new()
        where TDialogVM : class, INotifyPropertyChanged
    {
        Application.Current.Dispatcher.Invoke(
            () =>
            {
                if (ShowInner<TDialog>(out var dialog))
                {
                    var dialogVM = dialog.DataContext as TDialogVM;
                    var result = dialog.ShowDialog();
                    if (dialogVM is not null)
                    {
                        onOpened?.Invoke(dialogVM);
                        onClosed?.Invoke(result, dialogVM);
                    }
                }
            });
    }

    private static bool ShowInner<TDialog>([NotNullWhen(true)] out TDialog? dialog)
        where TDialog : Window, new()
    {
        var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        if (activeWindow is null)
        {
            dialog = null;
            return false;
        }
        else
        {
            dialog = new TDialog { Owner = activeWindow };
            return true;
        }
    }
}