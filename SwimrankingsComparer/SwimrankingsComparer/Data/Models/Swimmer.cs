using System.Text.Json.Serialization;

namespace SwimrankingsComparer.Data.Models;

public class Swimmer
{
    public Swimmer(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; set; } = Gender.Unknown;
    
    public string Club { get; set; } = string.Empty;
    
    public int YearOfBirth { get; set; }
    
    public IEnumerable<Pb> Pbs { get; set; } = new List<Pb>();
    
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}