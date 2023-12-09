using OctopusEnergyBillViewer.Model;
using OctopusEnergyBillViewer.Service;
using Reactive.Bindings;
using System.Reactive.Linq;

namespace OctopusEnergyBillViewer.Presentation.WPF.Dialogs;

internal class LoginDialogViewModel : ViewModelBase
{
    public ReactivePropertySlim<bool?> DialogResult { get; } = new();
    public ReactivePropertySlim<string> EmailAddress { get; } = new();
    public ReactivePropertySlim<string> Password { get; } = new();
    public ReactivePropertySlim<string> AccountNumber { get; } = new();

    public ReactiveCommand LoadedCmd { get; } = new();
    public ReactiveCommand OkCmd { get; }

    private readonly ApplicationService appService_ = Di.ApplicationService;

    public LoginDialogViewModel()
    {
        OkCmd = EmailAddress.Select(x => !string.IsNullOrEmpty(x))
            .CombineLatest(Password.Select(x => !string.IsNullOrEmpty(x)), (x, y) => x && y)
            .CombineLatest(AccountNumber.Select(x => !string.IsNullOrEmpty(x)), (x, y) => x && y)
            .ToReactiveCommand(false);

        LoadedCmd.Subscribe(
            async () =>
            {
                EmailAddress.Value = (await appService_.GetEmailAddressAsync()).Value;
                Password.Value = (await appService_.GetPasswordAsync()).Value;
                AccountNumber.Value = (await appService_.GetAccountNumberAsync()).Value;
            });
        OkCmd.Subscribe(
            async () =>
            {
                try
                {
                    await appService_.LoginAsync(new(EmailAddress.Value), new(Password.Value));
                    await appService_.SetAccountNumberAsync(new(AccountNumber.Value));
                    MessageBoxEx.Show("ログイン成功！");
                    DialogResult.Value = true;
                }
                catch (OctopusEnergyGraphqlException ex)
                {
                    switch (ex.Extensions?.errorType)
                    {
                        case "VALIDATION":
                            MessageBoxEx.Show("ログインに失敗しました。", "エラー", icon: System.Windows.MessageBoxImage.Error);
                            break;

                        default: throw;
                    }
                }
            });
    }
}