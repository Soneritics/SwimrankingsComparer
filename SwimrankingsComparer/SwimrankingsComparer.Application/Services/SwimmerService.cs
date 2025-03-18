using SwimRankings.Api;
using SwimRankings.Api.Models;
using SwimrankingsComparer.Application.Models;
using SwimrankingsComparer.Application.Repositories;

namespace SwimrankingsComparer.Application.Services;

public class SwimmerService(IRepository repository, ISwimmerApi swimmerApi)
{
    public async Task DeleteSwimmerAsync(string swimrankingsId) =>
        await repository.DeleteAsync<SwimmerData>(swimrankingsId);

    public async Task<IEnumerable<Swimmer>> GetAllSwimmersAsync() =>
        (await repository.GetListAsync<SwimmerData>(r => true))
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Select(s => new Swimmer(s.Id)
            {
                FirstName = s.FirstName,
                LastName = s.LastName,
            });
    
    public async Task<IEnumerable<SwimmerData>> GetAllSwimmerDataAsync() =>
        (await repository.GetListAsync<SwimmerData>(r => true))
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName);
    
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
        
        await UpdateHistory(swimrankingsId, swimmer);
        
        return swimmer;
    }

    public async Task<History?> GetHistoryAsync(string swimrankingsId) =>
        await repository.GetAsync<History>(swimrankingsId);

    private async Task<SwimmerData> GetSwimmerDataFromSource(string swimrankingsId) =>
        await swimmerApi.GetAsync(swimrankingsId);

    private async Task UpdateHistory(string swimrankingsId, SwimmerData swimmer)
    {
        var historyExists = await repository.ExistsAsync<History>(swimrankingsId);
        var history = historyExists
            ? await repository.GetAsync<History>(swimrankingsId)
            : new History(swimrankingsId, swimmer.FirstName, swimmer.LastName);

        foreach (var pb in swimmer.Pbs)
        {
            if (!history.HistoryCollection.Any(
                h => h.Stroke.Equals(pb.Stroke)
                    && h.DistanceInMeters == pb.DistanceInMeters
                    && h.PoolLength.Equals(pb.PoolLength)))
            {
                history.HistoryCollection.Add(new HistoryStrokeIdentifier(
                    pb.Stroke,
                    pb.DistanceInMeters,
                    pb.PoolLength));
            }

            var historyStrokeIdentifier = history.HistoryCollection.Single(
                h => h.Stroke.Equals(pb.Stroke)
                     && h.DistanceInMeters == pb.DistanceInMeters
                     && h.PoolLength.Equals(pb.PoolLength));

            if (!historyStrokeIdentifier.History.Any(h => h.SwimTime.TimeInMs < pb.SwimTime.TimeInMs))
            {
                historyStrokeIdentifier.History.Add(new PbOnDate(
                    pb.Meet?.Date ?? new Date(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    pb.SwimTime));
            }
        }
        
        if (historyExists)
        {
            await repository.UpdateAsync(swimrankingsId, history);
        }
        else
        {
            await repository.InsertAsync(swimrankingsId, history);
        }
    }
}