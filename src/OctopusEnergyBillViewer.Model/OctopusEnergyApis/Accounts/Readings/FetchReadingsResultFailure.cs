namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;

public record class FetchReadingsResultFailure(FetchReadingsResultFailureReason Reason) : FetchReadingsResult;
