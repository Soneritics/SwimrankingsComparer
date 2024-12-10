using SwimrankingsComparer.Data.Models;
using SwimrankingsComparer.Data.Repositories;

namespace SwimrankingsComparer.Data.Services;

public class SwimmerService(IRepository repository, HttpClient httpClient)
{
    public async Task<IEnumerable<Swimmer>> GetAllSwimmersAsync()
    {
        return (await repository.GetListAsync<Swimmer>(r => true))
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName);
    }
    
    public async Task<Swimmer> GetSwimmerAsync(string swimrankingsId, bool forceReload = false)
    {
        var swimmerExists = await repository.ExistsAsync<Swimmer>(swimrankingsId);
        if (!forceReload && swimmerExists)
        {
            return await repository.GetAsync<Swimmer>(swimrankingsId);
        }
        
        var swimmer = await GetSwimmerFromSource(swimrankingsId);
        if (swimmerExists)
        {
            await repository.UpdateAsync(swimrankingsId, swimmer);   
        }
        else
        {
            await repository.InsertAsync(swimrankingsId, swimmer);
        }
        
        return swimmer;
    }

    private async Task<Swimmer> GetSwimmerFromSource(string swimrankingsId)
    {
        var pageContents = await GetSwimmerPageContentsAsync(swimrankingsId);
        var swimmer = new Swimmer(swimrankingsId)
            .WithAthleteDetails(pageContents)
            .WithClub(pageContents)
            .WithGender(pageContents)
            .WithPbs(pageContents);

        return swimmer;
    }
    
    private Task<string> GetSwimmerPageContentsAsync(string swimrankingsId)
    {
        var url = $"https://www.swimrankings.net/index.php?page=athleteDetail&athleteId={swimrankingsId}&language=us";
        return httpClient.GetStringAsync(url);
    }
}
