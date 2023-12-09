namespace OctopusEnergyBillViewer.Model.Settings;
public record class Setting(EmailAddress EmailAddress, Password Password, AccessToken AccessToken, RefreshToken RefreshToken, AccountNumber AccountNumber);
