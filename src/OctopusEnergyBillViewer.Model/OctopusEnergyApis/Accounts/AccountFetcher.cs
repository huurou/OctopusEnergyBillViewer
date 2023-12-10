using OctopusEnergyBillViewer.Model.Accounts;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;
using System.Collections.ObjectModel;

namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts;

public class AccountFetcher(CredentialManager credentialManager, IOctpusEnergyApi api)
{
    public async Task<FetchReadingsResult> FetchHalfHourReadings(DateTime from, DateTime to)
    {
        try
        {
            return new FetchReadingsResultSuccess(await ObtainReadings());
        }
        catch (OctopusEnergyGraphqlException ex1)
        {
            // ヘッダーにAuthorizationがない or アクセストークンが異常
            if (ex1.Extensions?.errorType == "AUTHORIZATION" ||
            // AccessToken期限切れ
                ex1.Extensions?.errorType == "APPLICATION")
            {
                try
                {
                    await credentialManager.RefreshAccessToken();
                    return new FetchReadingsResultSuccess(await ObtainReadings());
                }
                catch (OctopusEnergyGraphqlException ex2)
                {
                    // リフレッシュトークンが異常
                    if (ex2.Extensions?.errorType == "VALIDATION")
                    {
                        try
                        {
                            await credentialManager.LoginAsync();
                            return new FetchReadingsResultSuccess(await ObtainReadings());
                        }
                        catch (OctopusEnergyGraphqlException ex3)
                        {
                            // ログイン情報が異常
                            if (ex3.Extensions?.errorType == "VALIDATION")
                            {
                                return new FetchReadingsResultFailure(FetchReadingsResultFailureReason.InvalidLoginInfo);
                            }
                            else throw;
                        }
                    }
                    else throw;
                }
            }
            else throw;
        }

        async Task<ReadOnlyCollection<HalfHourlyReading>> ObtainReadings()
        {
            return await api.ObtainHalfHourlyReadings(await credentialManager.GetAccessTokenAsync(), await credentialManager.GetAccountNumberAsync(), from, to);
        }
    }
}