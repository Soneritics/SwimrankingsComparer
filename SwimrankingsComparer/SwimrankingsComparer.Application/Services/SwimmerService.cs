using SwimrankingsComparer.Application.Helpers;
using SwimrankingsComparer.Application.Models;
using SwimrankingsComparer.Application.Repositories;

namespace SwimrankingsComparer.Application.Services;

public class SwimmerService(IRepository repository, HttpClient httpClient)
{
    public async Task<IEnumerable<Swimmer>> GetAllSwimmersAsync()
    {
        return (await repository.GetListAsync<SwimmerData>(r => true))
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Select(s => new Swimmer(s.Id)
            {
                FirstName = s.FirstName,
                LastName = s.LastName,
            });
    }
    
    public async Task<IEnumerable<SwimmerData>> GetAllSwimmerDataAsync()
    {
        return (await repository.GetListAsync<SwimmerData>(r => true))
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName);
    }
    
    public async Task<SwimmerData> GetSwimmerDataAsync(string swimrankingsId, bool forceReload = false)
    {
        var swimmerExists = await repository.ExistsAsync<SwimmerData>(swimrankingsId);
        if (!forceReload && swimmerExists)
        {
            return await repository.GetAsync<SwimmerData>(swimrankingsId);
        }
        
        var swimmer = await GetSwimmerDataFromSource(swimrankingsId);
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

    private async Task<SwimmerData> GetSwimmerDataFromSource(string swimrankingsId)
    {
        var pageContents = await GetSwimRankingsPageContentsAsync(swimrankingsId);
        var swimmer = new SwimmerData(swimrankingsId)
            .WithAthleteDetails(pageContents)
            .WithClub(pageContents)
            .WithGender(pageContents)
            .WithPbs(pageContents);

        return swimmer;
    }
    
    private Task<string> GetSwimRankingsPageContentsAsync(string swimrankingsId) =>
        httpClient.GetStringAsync(SwimrankingsUrlHelper.Get(swimrankingsId));
}
