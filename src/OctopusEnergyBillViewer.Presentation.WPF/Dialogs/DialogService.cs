using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace OctopusEnergyBillViewer.Presentation.WPF.Dialogs;

public class DialogService<TDialog>(IServiceProvider provider) : IDialogService<TDialog> where TDialog : Window
{
    public bool? ShowDialog()
    {
        var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        var dialog = provider.GetRequiredService<TDialog>();
        dialog.Owner = activeWindow;
        return dialog.ShowDialog();
    }
}
