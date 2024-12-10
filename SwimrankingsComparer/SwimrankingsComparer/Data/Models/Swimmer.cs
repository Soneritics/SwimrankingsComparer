using System.Text.Json.Serialization;

namespace SwimrankingsComparer.Data.Models;

public class Swimmer(string id)
{
    public string Id { get; set; } = id;

    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; set; } = Gender.Unknown;
    
    public string Club { get; set; } = string.Empty;
    
    public int YearOfBirth { get; set; }
    
    public List<Pb> Pbs { get; set; } = new List<Pb>();
    
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}