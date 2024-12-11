using SwimrankingsComparer.Application.Models;

namespace SwimrankingsComparer.Application.Extensions;

public static class StrokeExtensions
{
    public static Stroke ToStroke(this string stroke)
    {
        return stroke.ToLowerInvariant() switch
        {
            "freestyle" => Stroke.Freestyle,
            "backstroke" => Stroke.Backstroke,
            "breaststroke" => Stroke.Breaststroke,
            "butterfly" => Stroke.Butterfly,
            "medley" => Stroke.Medley,
            _ => Stroke.Unknown
        };
    }
}