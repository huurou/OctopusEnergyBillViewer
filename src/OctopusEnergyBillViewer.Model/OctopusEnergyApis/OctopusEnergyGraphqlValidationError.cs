namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis;

#pragma warning disable IDE1006 // 命名スタイル

public record OctopusEnergyGraphqlValidationError(string message, string[] inputPath);

#pragma warning restore IDE1006 // 命名スタイル
