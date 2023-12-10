namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;

public record class HalfHourlyReadingData(DateTime StartAt, DateTime EndAt, string Value, string CostEstimate)
{
    public HalfHourlyReading ToModel()
    {
        return new(StartAt, EndAt, new(decimal.Parse(Value)), new(decimal.Parse(CostEstimate)));
    }
}
