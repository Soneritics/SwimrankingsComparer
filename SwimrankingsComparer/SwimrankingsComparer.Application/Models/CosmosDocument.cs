using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace SwimrankingsComparer.Application.Models;

public class CosmosDocument<T>
{
    [JsonPropertyName("id")]
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = null!;

    public string Type { get; set; } = typeof(T).Name;

    public T? Data { get; set; }
}