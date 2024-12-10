using System.Net;
using System.Text.RegularExpressions;
using SwimrankingsComparer.Data.Helpers;
using SwimrankingsComparer.Data.Models;

namespace SwimrankingsComparer.Data.Services;

public static class SwimmerBuilder
{
    public static Swimmer WithAthleteDetails(this Swimmer swimmer, string pageContents)
    {
        var firstName = "Unknown";
        var lastName = "Unknown";
        int yearOfBirth = 0;
    
        var athleteMatch = RegexHelper.GetMatchValue(pageContents, @"<div id=""name"">(.*?)&");
    
        yearOfBirth = int.Parse(RegexHelper.GetMatchValue(athleteMatch, @"<br>\((.*?)$", yearOfBirth.ToString()));
        lastName = RegexHelper.GetMatchValue(athleteMatch, @"(.*?),", lastName).Trim();
        firstName = RegexHelper.GetMatchValue(athleteMatch, @",(.*?)<br>", firstName).Trim();

        swimmer.FirstName = firstName;
        swimmer.LastName = lastName;
        swimmer.YearOfBirth = yearOfBirth;
        
        return swimmer;
    }

    public static Swimmer WithClub(this Swimmer swimmer, string pageContents)
    {
        var matchClub = RegexHelper.GetMatchValue(pageContents, @"<div id=""nationclub""><br>(.*?)</div>");
        swimmer.Club = RegexHelper.GetMatchValue(matchClub, @"<br>(.*?)$");

        return swimmer;
    }

    public static Swimmer WithGender(this Swimmer swimmer, string pageContents)
    {
        var gender = Gender.Unknown;
        
        if (pageContents.Contains("athletes/athletemale.png"))
        {
            gender = Gender.Male;
        }
    
        if (pageContents.Contains("athletes/athletefemale.png"))
        {
            gender = Gender.Female;
        }

        swimmer.Gender = gender;

        return swimmer;
    }

    public static Swimmer WithPbs(this Swimmer swimmer, string pageContents)
    {
        swimmer.Pbs = new List<Pb>();
        
        var pbTable = RegexHelper.GetMatchValue(
            Regex.Replace(pageContents, @"escape\('(.*?)'\)", string.Empty),
            @"<table class=""athleteBest""(.*?)</table>");

        var pbLines = Regex.Matches(pbTable, @"<tr class=""athleteBest(.*?)</tr>");
        if (pbLines.Any())
        {
            foreach (Match pbLineMatch in pbLines)
            {
                var pbLine = pbLineMatch.Groups[1].Value.Trim();
                swimmer.Pbs.Add(CreatePbFromLine(pbLine));
            }
        }

        return swimmer;
    }
    
    private static Pb CreatePbFromLine(string pbLine)
    {
        var strokeAndDistance = RegexHelper.GetMatchValue(pbLine, @"<td class=""event""><a.*?>(.*?)<");
        var stroke = RegexHelper.GetMatchValue(strokeAndDistance, @"m (.*?)$");
        var distance = RegexHelper.GetMatchValue(strokeAndDistance, @"(.*?)m");
        var poolLength = RegexHelper.GetMatchValue(pbLine, @"<td class=""course"">(.*?)m</td>");
        var timeString = RegexHelper.GetMatchValue(pbLine, @"a  class=""time"".*?>(.*?)<");

        var meetName = RegexHelper.GetMatchValue(pbLine, @"<td class=""name"">.*?title=""(.*?)""");
        var meetDate = RegexHelper.GetMatchValue(pbLine, @"<td class=""date"">(.*?)</td>");
        var meetCity = RegexHelper.GetMatchValue(pbLine, @"<td class=""city"">.*?title="".*?"">(.*?)<");
        
        return new ()
        {
            Stroke = stroke.ToStroke(),
            DistanceInMeters = int.TryParse(distance, out var distanceValue) ? distanceValue : 0,
            PoolLength = int.TryParse(poolLength, out var poolLengthValue) ? poolLengthValue : 0,
            TimeInMs = timeString.ToTimeInMs(),
            Meet = new ()
            {
                Name = WebUtility.HtmlDecode(meetName),
                Date = WebUtility.HtmlDecode(meetDate),
                City = WebUtility.HtmlDecode(meetCity)
            }
        };
    }
}

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

public static class TimeStringExtensions
{
    public static int ToTimeInMs(this string timeString)
    {
        var timeParts = timeString.Split(':');
        
        var minutes = 0;

        if (timeParts.Length == 2)
        {
            if (!int.TryParse(timeParts[0], out minutes))
            {
                return 0;
            }
        }

        var secondsAndMs = timeParts.Last().Split('.');
        if (secondsAndMs.Length != 2)
        {
            return 0;
        }

        if (!int.TryParse(secondsAndMs[0], out var seconds))
        {
            return 0;
        }

        if (!int.TryParse(secondsAndMs[1], out var ms))
        {
            return 0;
        }

        return (minutes * 60 + seconds) * 1000 + ms;
    }
}