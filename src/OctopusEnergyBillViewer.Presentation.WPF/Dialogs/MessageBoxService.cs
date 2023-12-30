using System.Windows;

namespace OctopusEnergyBillViewer.Presentation.WPF.Dialogs;

public class MessageBoxService : IMessageBoxService
{
    public void Show(
        string message,
        string title = "",
        MessageBoxButton button = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None)
    {
        MessageBoxEx.Show(message, title, button, icon);
    }

    public void Show(
        Window? owner,
        string message,
        string title = "",
        MessageBoxButton button = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None)
    {
        MessageBoxEx.Show(owner, message, title, button, icon);
    }
}
