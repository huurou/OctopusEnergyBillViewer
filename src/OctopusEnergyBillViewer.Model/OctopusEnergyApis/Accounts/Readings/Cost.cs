namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;

/// <summary>
/// 推定電気代 単位：円
/// </summary>
/// <param name="Value">値</param>
public record class Cost(decimal Value);
