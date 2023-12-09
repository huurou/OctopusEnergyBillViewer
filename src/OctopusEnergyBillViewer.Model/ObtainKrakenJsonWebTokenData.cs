namespace OctopusEnergyBillViewer.Model;

public record class ObtainKrakenJsonWebTokenData(string Token, string RefreshToken)
{
    public ObtainKrakenJsonWebToken ToModel()
    {
        return new(new(Token), new(RefreshToken));
    }
}