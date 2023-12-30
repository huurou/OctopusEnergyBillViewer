using OctopusEnergyBillViewer.Model;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;
using OctopusEnergyBillViewer.Presentation.WPF.Dialogs;
using Reactive.Bindings;

namespace OctopusEnergyBillViewer.Presentation.WPF;

public class MainWindowViewModel : ViewModelBase
{
    public ReactivePropertySlim<decimal> UsageYesterday { get; } = new();
    public ReactivePropertySlim<decimal> CostYesterday { get; } = new();

    public ReactiveCommand LoadedCmd { get; } = new();
    public ReactiveCommand YesterdayCmd { get; } = new();

    private readonly IDialogService<LoginDialog> loginDialogService_;

    public MainWindowViewModel(IDialogService<LoginDialog> loginDialogService, ApplicationService appService)
    {
        loginDialogService_ = loginDialogService;

        LoadedCmd.Subscribe(
            async () =>
            {
                // 起動時にログイン情報かお客様番号がなければログインしてもらう
                if (string.IsNullOrEmpty((await appService.GetEmailAddressAsync()).Value) ||
                    string.IsNullOrEmpty((await appService.GetPasswordAsync()).Value) ||
                    string.IsNullOrEmpty((await appService.GetAccountNumberAsync()).Value))
                {
                    loginDialogService.ShowDialog();
                }
            });
        YesterdayCmd.Subscribe(
            async () =>
            {
                var result = await appService.FetchReadings(DateTime.Today.AddDays(-1), DateTime.Today);
                if (result is FetchReadingsResultSuccess success)
                {
                    UsageYesterday.Value = success.Readings.Sum(x => x.Usage.Value);
                    CostYesterday.Value = success.Readings.Sum(x => x.Cost.Value);
                }
                else if (result is FetchReadingsResultFailure failure)
                {
                    ProcessFetchReadingsResultFailure(failure);
                }
            });
    }

    private void ProcessFetchReadingsResultFailure(FetchReadingsResultFailure failure)
    {
        switch (failure.Reason)
        {
            case FetchReadingsResultFailureReason.NoAccountNumber:
            case FetchReadingsResultFailureReason.InvalidLoginInfo:
                loginDialogService_.ShowDialog();
                break;

            default:
                break;
        }
    }
}