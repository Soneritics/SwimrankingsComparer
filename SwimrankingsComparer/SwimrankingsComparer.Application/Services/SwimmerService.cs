using SwimRankings.Api;
using SwimRankings.Api.Models;
using SwimrankingsComparer.Application.Repositories;

namespace SwimrankingsComparer.Application.Services;

public class SwimmerService(IRepository repository, ISwimmerApi swimmerApi)
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

    private async Task<SwimmerData> GetSwimmerDataFromSource(string swimrankingsId) =>
        await swimmerApi.GetAsync(swimrankingsId);
}
