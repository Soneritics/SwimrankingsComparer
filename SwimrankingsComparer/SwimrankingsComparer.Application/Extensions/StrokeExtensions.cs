using SwimRankings.Api.Models;

namespace SwimrankingsComparer.Application.Extensions;

public static class StrokeExtensions
{
    public static string ToDutch(this Stroke stroke)
    {
        return stroke switch
        {
            Stroke.Butterfly => "Vlinderslag",
            Stroke.Backstroke => "Rugslag",
            Stroke.Breaststroke => "Schoolslag",
            Stroke.Freestyle => "Vrije slag",
            Stroke.Medley => "Wisselslag",
            Stroke.ButterflyLap => "Vlinderslag (lap)",
            Stroke.BackstrokeLap => "Rugslag (lap)",
            Stroke.BreaststrokeLap => "Schoolslag (lap)",
            Stroke.FreestyleLap => "Vrije slag (lap)",
            _ => "Onbekend"
        };
    }
}