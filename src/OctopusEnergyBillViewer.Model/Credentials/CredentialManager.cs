using OctopusEnergyBillViewer.Model.Credentials;
using OctopusEnergyBillViewer.Model.Settings;

namespace OctopusEnergyBillViewer.Model.Accounts;

public class CredentialManager(IOctopusEnergyApi api, ISettingRepository settingRepository)
{
    public async Task<EmailAddress> GetEmailAddressAsync()
    {
        var setting = await settingRepository.LoadAsync();
        return setting.EmailAddress;
    }

    public async Task SetEmailAddressAsync(EmailAddress email)
    {
        var setting = await settingRepository.LoadAsync();
        setting = setting with { EmailAddress = email };
        await settingRepository.SaveAsync(setting);
    }

    public async Task<Password> GetPasswordAsync()
    {
        var setting = await settingRepository.LoadAsync();
        return setting.Password;
    }

    public async Task SetPasswordAsync(Password password)
    {
        var setting = await settingRepository.LoadAsync();
        setting = setting with { Password = password };
        await settingRepository.SaveAsync(setting);
    }

    public async Task<AccountNumber> GetAccountNumberAsync()
    {
        var setting = await settingRepository.LoadAsync();
        return setting.AccountNumber;
    }

    public async Task SetAccountNumberAsync(AccountNumber accountNumber)
    {
        var setting = await settingRepository.LoadAsync();
        setting = setting with { AccountNumber = accountNumber };
        await settingRepository.SaveAsync(setting);
    }

    public async Task<RefreshToken> GetRefreshTokenAsync()
    {
        var setting = await settingRepository.LoadAsync();
        return setting.RefreshToken;
    }

    private async Task SetRefreshTokenAsync(RefreshToken refreshToken)
    {
        var setting = await settingRepository.LoadAsync();
        setting = setting with { RefreshToken = refreshToken };
        await settingRepository.SaveAsync(setting);
    }

    public async Task<AccessToken> GetAccessTokenAsync()
    {
        var setting = await settingRepository.LoadAsync();
        return setting.AccessToken;
    }

    private async Task SetAccessTokenAsync(AccessToken value)
    {
        var setting = await settingRepository.LoadAsync();
        setting = setting with { AccessToken = value };
        await settingRepository.SaveAsync(setting);
    }

    public async Task LoginAsync()
    {
        await LoginAsync(await GetEmailAddressAsync(), await GetPasswordAsync());
    }

    public async Task LoginAsync(EmailAddress email, Password password)
    {
        var jwt = await api.ObtainKrakenTokenAsync(email, password);
        await SetEmailAddressAsync(email);
        await SetPasswordAsync(password);
        await SetAccessTokenAsync(jwt.Token);
        await SetRefreshTokenAsync(jwt.RefreshToken);
    }

    public async Task RefreshAccessToken()
    {
        var jwt = await api.ObtainKrakenTokenAsync(await GetRefreshTokenAsync());
        await SetAccessTokenAsync(jwt.Token);
        await SetRefreshTokenAsync(jwt.RefreshToken);
    }
}