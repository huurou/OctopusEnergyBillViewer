using OctopusEnergyBillViewer.Model;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Windows;

namespace OctopusEnergyBillViewer.Presentation.WPF.Dialogs;

public class LoginDialogViewModel : ViewModelBase
{
    public ReactivePropertySlim<bool?> DialogResult { get; } = new();
    public ReactivePropertySlim<string> EmailAddress { get; } = new();
    public ReactivePropertySlim<string> Password { get; } = new();
    public ReactivePropertySlim<string> AccountNumber { get; } = new();

    public ReactiveCommand LoadedCmd { get; } = new();
    public ReactiveCommand OkCmd { get; }

    public LoginDialogViewModel(IMessageBoxService messageBoxService, ApplicationService appService)
    {
        OkCmd = EmailAddress.Select(x => !string.IsNullOrEmpty(x))
            .CombineLatest(Password.Select(x => !string.IsNullOrEmpty(x)), (x, y) => x && y)
            .CombineLatest(AccountNumber.Select(x => !string.IsNullOrEmpty(x)), (x, y) => x && y)
            .ToReactiveCommand(false);

        LoadedCmd.Subscribe(
            async () =>
            {
                EmailAddress.Value = (await appService.GetEmailAddressAsync()).Value;
                Password.Value = (await appService.GetPasswordAsync()).Value;
                AccountNumber.Value = (await appService.GetAccountNumberAsync()).Value;
            });
        OkCmd.Subscribe(
            async () =>
            {
                try
                {
                    await appService.LoginAsync(new(EmailAddress.Value), new(Password.Value));
                    await appService.SetAccountNumberAsync(new(AccountNumber.Value));
                    messageBoxService.Show("ログイン成功！");
                    DialogResult.Value = true;
                }
                catch (OctopusEnergyGraphqlException ex)
                {
                    switch (ex.Extensions?.errorType)
                    {
                        case "VALIDATION":
                            messageBoxService.Show("ログインに失敗しました。", "エラー", icon: MessageBoxImage.Error);
                            break;

                        default: throw;
                    }
                }
            });
    }
}