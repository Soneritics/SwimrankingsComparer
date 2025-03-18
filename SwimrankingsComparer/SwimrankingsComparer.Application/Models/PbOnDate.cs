using SwimRankings.Api.Models;

namespace SwimrankingsComparer.Application.Models;

public class PbOnDate(Date date, SwimTime swimTime)
{
    public Date Date { get; set; } = date;
    public SwimTime SwimTime { get; set; } = swimTime;
}