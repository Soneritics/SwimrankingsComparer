using System.Net;
using Microsoft.Azure.Cosmos;
using SwimrankingsComparer.Application.Models;

namespace SwimrankingsComparer.Application.Repositories;

public class CosmosRepository : IRepository
{
    private readonly Database _database;
    private readonly Dictionary<string, Container> _containers = new ();

    public CosmosRepository(string connectionString, string databaseName)
    {
        var cosmosClient = new CosmosClient(connectionString);
        _database = cosmosClient.GetDatabase(databaseName);
    }

    public async Task<bool> ExistsAsync<T>(string id)
    {
        try
        {
            await GetAsync<T>(id);
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task InsertAsync<T>(string id, T entity)
    {
        var doc = new CosmosDocument<T>()
        {
            Id = id,
            Data = entity
        };

        await GetContainer<T>().UpsertItemAsync(doc, new PartitionKey(id));
    }

    public async Task UpdateAsync<T>(string id, T entity)
    {
        var doc = new CosmosDocument<T>()
        {
            Id = id,
            Data = entity
        };

        await GetContainer<T>().UpsertItemAsync(doc, new PartitionKey(id));
    }

    public async Task<T> GetAsync<T>(string id)
    {
        var result = await GetContainer<T>()
            .ReadItemAsync<CosmosDocument<T>>(id, new PartitionKey(id));

        return result.Resource.Data!;
    }

    public async Task DeleteAsync<T>(string id)
    {
        await GetContainer<T>().DeleteItemAsync<CosmosDocument<T>>(id, new PartitionKey(id));
    }

    public Task<List<T>> GetListAsync<T>(Func<T, bool> predicate) =>
        Task.FromResult(
            GetContainer<T>()
                .GetItemLinqQueryable<CosmosDocument<T>>(true)
                .Where(r => r.Type.Equals(typeof(T).Name))
                .Select(r => r.Data)!
                .Where(predicate)
                .ToList());
    
    private Container GetContainer<T> ()
    {
        var containerName = typeof(T).Name;
        if (!_containers.ContainsKey(containerName))
        {
            _database.CreateContainerIfNotExistsAsync(containerName, "/id");
            _containers.Add(containerName, _database.GetContainer(containerName));
        }

        return _containers[containerName];
    }
}