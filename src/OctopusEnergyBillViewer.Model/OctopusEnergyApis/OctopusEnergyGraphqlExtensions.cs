namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis;
#pragma warning disable IDE1006 // 命名スタイル

public record OctopusEnergyGraphqlExtensions(string errorType, string errorCode, string errorDescription, string errorClass, List<OctopusEnergyGraphqlValidationError> validationErrors);

#pragma warning restore IDE1006 // 命名スタイル
