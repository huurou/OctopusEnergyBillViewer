using System.Windows;

namespace OctopusEnergyBillViewer.Presentation.WPF.Dialogs;

public interface IDialogService<TDialog>
    where TDialog : Window
{
    bool? ShowDialog();
}
