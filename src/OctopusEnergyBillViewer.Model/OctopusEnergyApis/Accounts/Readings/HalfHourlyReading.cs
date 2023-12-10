namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;

public record class HalfHourlyReading(DateTime StartAt, DateTime EndAt, Usage Usage, Cost Cost);
