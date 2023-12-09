using OctopusEnergyBillViewer.Model.Events;
using System.Collections.ObjectModel;

namespace OctopusEnergyBillViewer.Model;

public abstract record class FetchReadingsResult;

public record class FetchReadingsResultSuccess(ReadOnlyCollection<HalfHourlyReading> Readings) : FetchReadingsResult
{
}

public record class FetchReadingsResultFailure(FetchReadingsResultFailureReason Reason) : FetchReadingsResult
{
}

public enum FetchReadingsResultFailureReason
{
    Unknown,
    NoAccountNumber,
    InvalidLoginInfo,
}

public class ApplicationService
{
    /// <summary>
    /// ログイン情報入力要求イベント
    /// </summary>
    public Event LoginInfoInputRequest { get; } = new();
    public Event AccountNumberInputRequest { get; } = new();

    private readonly CredentialManager credentialManager_;
    private readonly IOctpusEnergyApi api_;

    public ApplicationService(CredentialManager credentialManager, IOctpusEnergyApi api)
    {
        credentialManager_ = credentialManager;
        api_ = api;

        credentialManager.LoginInfoInputRequest.Subscribe(LoginInfoInputRequest.Publish);
    }

    public async Task<EmailAddress> GetEmailAddressAsync()
    {
        return await credentialManager_.GetEmailAddressAsync();
    }

    public async Task<Password> GetPasswordAsync()
    {
        return await credentialManager_.GetPasswordAsync();
    }

    public async Task<AccountNumber> GetAccountNumberAsync()
    {
        return await credentialManager_.GetAccountNumberAsync();
    }

    public async Task SetAccountNumberAsync(AccountNumber accountNumber)
    {
        await credentialManager_.SetAccountNumberAsync(accountNumber);
    }

    public async Task<RefreshToken> GetRefreshTokenAsync()
    {
        return await credentialManager_.GetRefreshTokenAsync();
    }

    public async Task LoginAsync()
    {
        await credentialManager_.LoginAsync(await GetEmailAddressAsync(), await GetPasswordAsync());
    }

    public async Task LoginAsync(EmailAddress email, Password password)
    {
        await credentialManager_.LoginAsync(email, password);
    }

    public async Task<FetchReadingsResult> FetchReadings(DateTime from, DateTime to)
    {
        var accountNumber = await credentialManager_.GetAccountNumberAsync();
        var accessToken = await credentialManager_.GetAccessTokenAsync();
        try
        {
            var readings = await api_.ObtainHalfHourlyReadings(accessToken, accountNumber, from, to);
            return new FetchReadingsResultSuccess(readings);
        }
        catch (OctopusEnergyGraphqlException ex1)
        {
            // ヘッダーにAuthorizationがない
            if (ex1.Extensions?.errorType == "AUTHORIZATION" && ex1.Extensions?.errorCode == "KT-CT-1112" ||
            // AccessToken期限切れ
                ex1.Extensions?.errorType == "APPLICATION" && ex1.Extensions?.errorCode == "KT-CT-1124")
            {
                try
                {
                    await credentialManager_.RefreshAccessToken();
                }
                catch (OctopusEnergyGraphqlException ex2)
                {
                    // リフレッシュトークンが異常
                    if (ex2.Extensions?.errorType == "VALIDATION" && ex2.Extensions?.errorCode == "KT-CT-1137")
                    {
                        try
                        {
                            await credentialManager_.LoginAsync();
                        }
                        catch (OctopusEnergyGraphqlException ex3)
                        {
                            // ログイン情報が異常
                            if (ex3.Extensions?.errorType == "VALIDATION" && ex3.Extensions?.errorCode == "KT-CT-1137")
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
        return new FetchReadingsResultFailure(FetchReadingsResultFailureReason.Unknown);
    }
}