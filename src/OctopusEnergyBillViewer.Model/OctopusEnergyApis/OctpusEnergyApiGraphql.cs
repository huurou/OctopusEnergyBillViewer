﻿using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using OctopusEnergyBillViewer.Model.Accounts;
using OctopusEnergyBillViewer.Model.Credentials;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts.Readings;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.KrakenTokens;
using System.Collections.ObjectModel;

namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis;

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
                fromDatetime = fromDatetime.ToString("O"),
                toDatetime = toDatetime.ToString("O"),
            },
            Authorization = accessToken.Value,
        };
        var response = await client.SendQueryAsync<AccountResponse>(request);
        return response.Errors is GraphQLError[] errors
            ? throw new OctopusEnergyGraphqlException(errors)
            : response.Data.Account.Properties.First().ElectricitySupplyPoints.First().HalfHourlyReadings.Select(x => x.ToModel()).ToList().AsReadOnly();
    }
}
