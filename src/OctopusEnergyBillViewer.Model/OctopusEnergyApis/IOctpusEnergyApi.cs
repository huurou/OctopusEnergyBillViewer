using OctopusEnergyBillViewer.Model.Accounts;
using OctopusEnergyBillViewer.Model.Credentials;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.KrakenTokens;
using System.Collections.ObjectModel;

namespace OctopusEnergyBillViewer.Model;

public interface IOctpusEnergyApi
{
    Task<ObtainKrakenJsonWebToken> ObtainKrakenTokenAsync(EmailAddress email, Password password);

    Task<ObtainKrakenJsonWebToken> ObtainKrakenTokenAsync(RefreshToken refreshToken);

    Task<ReadOnlyCollection<HalfHourlyReading>> ObtainHalfHourlyReadings(AccessToken accessToken, AccountNumber accountNumber, DateTime fromDatetime, DateTime toDatetime);
}