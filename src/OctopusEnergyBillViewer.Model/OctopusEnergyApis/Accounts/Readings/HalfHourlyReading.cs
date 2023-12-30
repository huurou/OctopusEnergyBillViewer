namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;

/// <summary>
/// 30分ごとの使用量と電気代
/// </summary>
/// <param name="StartAt">開始時刻</param>
/// <param name="EndAt">終了時刻</param>
/// <param name="Usage">使用量[kwh]</param>
/// <param name="Cost">電気代[円]</param>
public record class HalfHourlyReading(DateTime StartAt, DateTime EndAt, Usage Usage, Cost Cost);
