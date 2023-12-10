using System.Collections.ObjectModel;

namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;

public abstract record class FetchReadingsResult;
public record class FetchReadingsResultSuccess(ReadOnlyCollection<HalfHourlyReading> Readings) : FetchReadingsResult;
public record class FetchReadingsResultFailure(FetchReadingsResultFailureReason Reason) : FetchReadingsResult;

public enum FetchReadingsResultFailureReason
{
    NoAccountNumber,
    InvalidLoginInfo,
}
