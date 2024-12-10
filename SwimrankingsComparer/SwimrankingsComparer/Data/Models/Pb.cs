using System.Text.Json.Serialization;

namespace SwimrankingsComparer.Data.Models;

public class Pb
{
    public string Id => $"{Stroke}-{DistanceInMeters}-{BadLength}";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Stroke Stroke { get; set; }

    public int DistanceInMeters { get; set; }
    public int TimeInMs { get; set; }
    public int BadLength { get; set; }
    public Meet Meet { get; set; }
}