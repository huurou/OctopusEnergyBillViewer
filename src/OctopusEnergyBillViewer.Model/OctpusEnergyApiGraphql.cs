using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace OctopusEnergyBillViewer.Model;

public class OctpusEnergyApiGraphql : IOctpusEnergyApi
{
    private const string URL = "https://api.oejp-kraken.energy/v1/graphql/";

    public async Task<ObtainKrakenJsonWebToken> ObtainKrakenTokenAsync(EmailAddress email, Password password)
    {
        var client = new GraphQLHttpClient(URL, new SystemTextJsonSerializer());
        var request = new GraphQLRequest()
        {
            Query = """
            mutation krakenTokenAuthenticationByEmailAndPassword($email: String!, $password: String!) {
              obtainKrakenToken(input: { email: $email, password: $password}) {
                token
                refreshToken
              }
            }
            """,
            Variables = new
            {
                email = email.Value,
                password = password.Value,
            },
        };
        var response = await client.SendMutationAsync<ObtainKrakenTokenResponse>(request);
        return response.Errors is GraphQLError[] errors
            ? throw new OctopusEnergyGraphqlException(errors)
            : response.Data.ObtainKrakenToken.ToModel();
    }

    public async Task<ObtainKrakenJsonWebToken> ObtainKrakenTokenAsync(RefreshToken refreshToken)
    {
        var client = new GraphQLHttpClient(URL, new SystemTextJsonSerializer());
        var request = new GraphQLRequest()
        {
            Query = """
            mutation krakenTokenAuthenticationByRefreshToken($refreshToken: String!) {
              obtainKrakenToken(input: {refreshToken: $refreshToken}) {
                token
                refreshToken
              }
            }
            """,
            Variables = new { refreshToken = refreshToken.Value }
        };
        var response = await client.SendMutationAsync<ObtainKrakenTokenResponse>(request);
        return response.Errors is GraphQLError[] errors
            ? throw new OctopusEnergyGraphqlException(errors)
            : response.Data.ObtainKrakenToken.ToModel();
    }

    public async Task<ReadOnlyCollection<HalfHourlyReading>> ObtainHalfHourlyReadings(AccessToken accessToken, AccountNumber accountNumber, DateTime fromDatetime, DateTime toDatetime)
    {
        var client = new GraphQLHttpClient(URL, new SystemTextJsonSerializer());
        var request = new GraphQLHttpRequestWithAuthorization()
        {
            Query = """
            query halfHourlyReadings($accountNumber:String!, $fromDatetime: DateTime!, $toDatetime: DateTime!) {
              account(accountNumber: $accountNumber) {
                properties {
                  electricitySupplyPoints {
                    halfHourlyReadings(
                      fromDatetime: $fromDatetime
                      toDatetime: $toDatetime
                    ) {
                      startAt
                      endAt
                      value
                      costEstimate
                    }
                  }
                }
              }
            }
            """,
            Variables = new
            {
                accountNumber = accountNumber.Value,
                fromDatetime,
                toDatetime
            },
            Authorization = accessToken.Value,
        };
        var response = await client.SendQueryAsync<AccountResponse>(request);
        return response.Errors is GraphQLError[] errors
            ? throw new OctopusEnergyGraphqlException(errors)
            : response.Data.Account.Properties.First().ElectricitySupplyPoints.First().HalfHourlyReadings.Select(x => x.ToModel()).ToList().AsReadOnly();
    }
}

public class GraphQLHttpRequestWithAuthorization : GraphQLHttpRequest
{
    public string? Authorization { get; set; }

    public override HttpRequestMessage ToHttpRequestMessage(GraphQLHttpClientOptions options, IGraphQLJsonSerializer serializer)
    {
        var message = base.ToHttpRequestMessage(options, serializer);
        if (!string.IsNullOrEmpty(Authorization))
        {
            message.Headers.Add("Authorization", Authorization);
        }
        return message;
    }
}

public class OctopusEnergyGraphqlException(GraphQLError[] errors) : Exception
{
    private static readonly JsonSerializerOptions options_ = new(JsonSerializerDefaults.Web) { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public GraphQLError[] Errors { get; } = errors;

    public override string Message { get; } = JsonSerializer.Serialize(errors, options_);
    public OctopusEnergyGraphqlExtensions? Extensions { get; } = JsonSerializer.Deserialize<OctopusEnergyGraphqlExtensions>(JsonSerializer.Serialize(errors[0].Extensions));
}

#pragma warning disable IDE1006 // 命名スタイル

public record OctopusEnergyGraphqlExtensions(string errorType, string errorCode, string errorDescription, string errorClass, List<OctopusEnergyGraphqlValidationError> validationErrors);

public record OctopusEnergyGraphqlValidationError(string message, string[] inputPath);

#pragma warning restore IDE1006 // 命名スタイル

/// <summary>
/// 電気使用量 単位：kwh
/// </summary>
/// <param name="Value">値</param>
public record class Usage(double Value);
/// <summary>
/// 推定電気代 単位：円
/// </summary>
/// <param name="Value">値</param>
public record class Cost(double Value);
public record class HalfHourlyReading(DateTime StartAt, DateTime EndAt, Usage Usage, Cost Cost);

public record class AccountResponse(Account Account);
public record class Account(List<Property> Properties);
public record class Property(List<ElectricitySupplyPointData> ElectricitySupplyPoints);
public record class ElectricitySupplyPointData(List<HalfHourlyReadingData> HalfHourlyReadings);
public record class HalfHourlyReadingData(DateTime StartAt, DateTime EndAt, string Value, string CostEstimate)
{
    public HalfHourlyReading ToModel()
    {
        return new(StartAt, EndAt, new(double.Parse(Value)), new(double.Parse(CostEstimate)));
    }
}