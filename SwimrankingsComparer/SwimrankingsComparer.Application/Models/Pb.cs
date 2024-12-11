using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace SwimrankingsComparer.Application.Models;

public class Pb
{
    public string Id => $"{Stroke}-{DistanceInMeters}-{PoolLength}";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public Stroke Stroke { get; set; }

    public int DistanceInMeters { get; set; }
    public SwimTime SwimTime { get; set; }
    public int PoolLength { get; set; }
    public Meet? Meet { get; set; }
}