using SwimRankings.Api.Models;

namespace SwimrankingsComparer.Application.Models;

public class HistoryStrokeIdentifier(Stroke stroke, int distanceInMeters, int poolLength)
{
    public Stroke Stroke { get; set; } = stroke;
    public int DistanceInMeters { get; set; } = distanceInMeters;
    public int PoolLength { get; set; } = poolLength;
    public List<PbOnDate> History { get; set; } = new ();
}