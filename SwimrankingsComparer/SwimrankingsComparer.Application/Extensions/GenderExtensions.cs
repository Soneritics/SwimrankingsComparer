using SwimRankings.Api.Models;

namespace SwimrankingsComparer.Application.Extensions;

public static class GenderExtensions
{
    public static string ToDutch(this Gender gender)
    {
        return gender switch
        {
            Gender.Male => "Man",
            Gender.Female => "Vrouw",
            _ => "Onbekend"
        };
    }
}