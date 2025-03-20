using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly IServer? _server;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();

            var endpoint = _redis.GetEndPoints().FirstOrDefault();
            _server = endpoint != null ? _redis.GetServer(endpoint) : null;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                if (value.IsNullOrEmpty)
                {
                    return default;
                }

                var deserializedValue = JsonSerializer.Deserialize<T>(value!);
                return deserializedValue ?? default; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving key {key} from cache: {ex.Message}");
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var serialized = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key, serialized, expiry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting key {key} in cache: {ex.Message}");
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing key {key} from cache: {ex.Message}");
            }
        }
    }
}
