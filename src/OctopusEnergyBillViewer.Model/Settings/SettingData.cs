namespace OctopusEnergyBillViewer.Model.Settings;

public record class SettingData(string EmailAddress, string Password, string AccessToken, string RefreshToken, string AccountNumber)
{
    public static SettingData FromModel(Setting setting)
    {
        return new(setting.EmailAddress.Value, setting.Password.Value, setting.AccessToken.Value, setting.RefreshToken.Value, setting.AccountNumber.Value);
    }

    public Setting ToModel()
    {
        return new(new(EmailAddress), new(Password), new(AccessToken), new(RefreshToken), new(AccountNumber));
    }
}
