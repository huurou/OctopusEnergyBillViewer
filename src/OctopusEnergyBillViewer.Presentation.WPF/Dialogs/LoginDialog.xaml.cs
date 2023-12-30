using System.Windows;

namespace OctopusEnergyBillViewer.Presentation.WPF.Dialogs;

/// <summary>
/// LoginDialog.xaml の相互作用ロジック
/// </summary>
public partial class LoginDialog : Window
{
    public LoginDialog(LoginDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
