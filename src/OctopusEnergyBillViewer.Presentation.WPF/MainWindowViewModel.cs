using OctopusEnergyBillViewer.Model;
using OctopusEnergyBillViewer.Presentation.WPF.Dialogs;
using OctopusEnergyBillViewer.Service;
using Reactive.Bindings;

namespace OctopusEnergyBillViewer.Presentation.WPF;

public class MainWindowViewModel : ViewModelBase
{
    public ReactivePropertySlim<double> UsageYesterday { get; } = new();
    public ReactivePropertySlim<double> CostYesterday { get; } = new();

    public ReactiveCommand LoadedCmd { get; } = new();
    public ReactiveCommand YesterdayCmd { get; } = new();

    private readonly ApplicationService appService_ = Di.ApplicationService;

    public MainWindowViewModel()
    {
        LoadedCmd.Subscribe(
            async () =>
            {
                // 起動時にログイン情報かお客様番号がなければログインしてもらう
                if (string.IsNullOrEmpty((await appService_.GetEmailAddressAsync()).Value) ||
                    string.IsNullOrEmpty((await appService_.GetPasswordAsync()).Value) ||
                    string.IsNullOrEmpty((await appService_.GetAccountNumberAsync()).Value))
                {
                    DialogService.Show<LoginDialog, LoginDialogViewModel>();
                }
            });
        YesterdayCmd.Subscribe(
            async () =>
            {
                var result = await appService_.FetchReadings(DateTime.Today.AddDays(-1), DateTime.Today);
                if (result is FetchReadingsResultSuccess success)
                {
                    UsageYesterday.Value = success.Readings.Sum(x => x.Usage.Value);
                    CostYesterday.Value = success.Readings.Sum(x => x.Cost.Value);
                }
                else if (result is FetchReadingsResultFailure failure)
                {
                    switch (failure.Reason)
                    {
                        case FetchReadingsResultFailureReason.Unknown:
                            break;

                        case FetchReadingsResultFailureReason.NoAccountNumber:
                        case FetchReadingsResultFailureReason.InvalidLoginInfo:
                            DialogService.Show<LoginDialog, LoginDialogViewModel>();
                            break;

                        default:
                            break;
                    }
                }
            });
    }
}