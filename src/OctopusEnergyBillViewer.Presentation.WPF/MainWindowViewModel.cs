using LiveCharts;
using LiveCharts.Wpf;
using OctopusEnergyBillViewer.Model;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;
using OctopusEnergyBillViewer.Presentation.WPF.Dialogs;
using Reactive.Bindings;
using System.Reactive.Linq;

namespace OctopusEnergyBillViewer.Presentation.WPF;

public class MainWindowViewModel : ViewModelBase
{
    public ReactivePropertySlim<decimal> UsageYesterday { get; } = new();
    public ReactivePropertySlim<decimal> CostYesterday { get; } = new();

    public ReactivePropertySlim<SeriesCollection> SeriesCollection { get; } = new();
    public ReactivePropertySlim<string[]> Labels { get; } = new();
    public ReactivePropertySlim<Func<decimal, string>> Formatter { get; } = new();

    public ReactiveCommand LoadedCmd { get; } = new();
    public ReactiveCommand YesterdayCmd { get; } = new();
    public ReactiveCommand WeekCmd { get; } = new();

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
        WeekCmd.Subscribe(
            async () =>
            {
                var result = await appService.FetchReadings(DateTime.Today.AddDays(-7), DateTime.Today);
                if (result is FetchReadingsResultSuccess success)
                {
                    var labels = new List<string>();
                    var values = new ChartValues<decimal>();

                    var groups = success.Readings.GroupBy(x => new { x.StartAt.Year, x.StartAt.Month, x.StartAt.Day });
                    foreach (var group in groups)
                    {
                        labels.Add($"{group.Key.Year}/{group.Key.Month}/{group.Key.Day}");
                        values.Add(group.Sum(x => x.Usage.Value));
                    }

                    Labels.Value = labels.ToArray();
                    SeriesCollection.Value = new SeriesCollection()
                    {
                        new ColumnSeries() { Title = "使用量", Values = values }
                    };
                    
                    Formatter.Value = value => value.ToString("N");
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