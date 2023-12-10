using GraphQL;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace OctopusEnergyBillViewer.Model.OctopusEnergyApis;

public class OctopusEnergyGraphqlException(GraphQLError[] errors) : Exception
{
    private static readonly JsonSerializerOptions options_ = new(JsonSerializerDefaults.Web) { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public GraphQLError[] Errors { get; } = errors;

    public override string Message { get; } = JsonSerializer.Serialize(errors, options_);
    public OctopusEnergyGraphqlExtensions? Extensions { get; } = JsonSerializer.Deserialize<OctopusEnergyGraphqlExtensions>(JsonSerializer.Serialize(errors[0].Extensions));
}
