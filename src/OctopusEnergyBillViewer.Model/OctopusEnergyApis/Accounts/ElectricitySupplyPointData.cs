using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;

namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis;

public record class ElectricitySupplyPointData(List<HalfHourlyReadingData> HalfHourlyReadings);
