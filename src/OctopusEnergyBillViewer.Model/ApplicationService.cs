using OctopusEnergyBillViewer.Model.Accounts;
using OctopusEnergyBillViewer.Model.Credentials;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;

namespace OctopusEnergyBillViewer.Model;

public class ApplicationService(CredentialManager credentialManager, AccountFetcher accountFetcher)
{
    public async Task<EmailAddress> GetEmailAddressAsync()
    {
        return await credentialManager.GetEmailAddressAsync();
    }

    public async Task<Password> GetPasswordAsync()
    {
        return await credentialManager.GetPasswordAsync();
    }

    public async Task<AccountNumber> GetAccountNumberAsync()
    {
        return await credentialManager.GetAccountNumberAsync();
    }

    public async Task SetAccountNumberAsync(AccountNumber accountNumber)
    {
        await credentialManager.SetAccountNumberAsync(accountNumber);
    }

    public async Task<RefreshToken> GetRefreshTokenAsync()
    {
        return await credentialManager.GetRefreshTokenAsync();
    }

    public async Task LoginAsync()
    {
        await credentialManager.LoginAsync(await GetEmailAddressAsync(), await GetPasswordAsync());
    }

    public async Task LoginAsync(EmailAddress email, Password password)
    {
        await credentialManager.LoginAsync(email, password);
    }

    public async Task<FetchReadingsResult> FetchReadings(DateTime from, DateTime to)
    {
        return await accountFetcher.FetchHalfHourReadings(from, to);
    }
}
