using System.Text.Json.Serialization;

namespace SwimrankingsComparer.Data.Models.Cosmos;

public class CosmosDocument<T>
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    public string Type { get; set; } = typeof(T).Name;

    public T Data { get; set; }
}