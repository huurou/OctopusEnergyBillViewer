namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;

/// <summary>
/// 電気使用量 単位：kwh
/// </summary>
/// <param name="Value">値</param>
public record class Usage(decimal Value);
