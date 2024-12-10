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
            
                Console.WriteLine(pbLine);
            }
        }

        return swimmer;
    }
}
/*
    new ()
    {
        Stroke = Stroke.Freestyle,
        DistanceInMeters = 50,
        TimeInMs = 1000,
        BadLength = 0,
        Meet = new Meet()
        {
            Name = "Test Meet",
            Date = "2021-01-01",
            City = "Test City"
        }
    }
*/