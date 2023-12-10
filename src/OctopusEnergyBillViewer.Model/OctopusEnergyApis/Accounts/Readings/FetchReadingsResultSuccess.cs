using System.Collections.ObjectModel;

namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;

public record class FetchReadingsResultSuccess(ReadOnlyCollection<HalfHourlyReading> Readings) : FetchReadingsResult;
