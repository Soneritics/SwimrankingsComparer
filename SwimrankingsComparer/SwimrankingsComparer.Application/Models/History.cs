namespace SwimrankingsComparer.Application.Models;

public class History(string id, string firstName, string lastName)
{
    public string Id { get; set; } = id;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public List<HistoryStrokeIdentifier> HistoryCollection { get; set; } = new ();
}