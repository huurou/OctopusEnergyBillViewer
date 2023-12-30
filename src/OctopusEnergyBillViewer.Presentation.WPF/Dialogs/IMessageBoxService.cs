using System.Windows;

namespace OctopusEnergyBillViewer.Presentation.WPF.Dialogs;

public interface IMessageBoxService
{
    void Show(
        string message,
        string title = "",
        MessageBoxButton button = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None);

    void Show(
        Window? owner,
        string message,
        string title = "",
        MessageBoxButton button = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None);
}
