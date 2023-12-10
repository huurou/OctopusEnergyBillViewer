using OctopusEnergyBillViewer.Model.Accounts;

namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis.KrakenTokens;

public record class ObtainKrakenJsonWebToken(AccessToken Token, RefreshToken RefreshToken);

public record class ObtainKrakenJsonWebTokenData(string Token, string RefreshToken)
{
    public ObtainKrakenJsonWebToken ToModel()
    {
        return new(new(Token), new(RefreshToken));
    }
}