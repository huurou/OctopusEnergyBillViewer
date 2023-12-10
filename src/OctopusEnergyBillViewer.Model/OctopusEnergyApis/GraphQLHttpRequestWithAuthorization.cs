using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;

namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis;

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
