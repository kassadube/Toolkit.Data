using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Extensions;

namespace Toolkit.Data
{
    public class RedisService
    {
        IConnectionMultiplexer _connectionMultiplexer;
        public IDatabase _cache = null;
        ILogger _logger;
        public RedisService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisService> logger)
        {
            _connectionMultiplexer =  connectionMultiplexer;
            _logger = logger;
            ConnectDB();
        }

        private void ConnectDB()
        {
            try
            {
                var StartTime = DateTime.Now;
                var totalTime = new TimeSpan();
               
               // ConfigurationOptions options = ConfigurationOptions.Parse(Fleet.Configuration.ConfigManager.DBSETTINGS.RedisConnectionString);
               // options.ConnectTimeout = 100000;
               // options.SyncTimeout = 5000;
                _cache = _connectionMultiplexer.GetDatabase();
                totalTime = DateTime.Now - StartTime;
               // LogItem log = new LogItem() { LogType = LOGTYPE.Cache, Message = string.Format("CONNECT TO REDIS TIME: {0}", totalTime) };
              //  LoggingFactory.GetRedisLogger.Custom(log);

            }
            catch (Exception ex)
            {
               // ILogger logger = LoggingFactory.GetLogger;
                _logger.LogError("ERROR CONNECTING TO REDIS MESSAGE = {0}".StringFormat(ex.Message));
                throw;
            }
        }

        public async Task<Task<bool>> InsertHashAsync(string key, string hashKey, object item)
        {
            try
            {
                var StartTime = DateTime.Now;
                var totalTime = new TimeSpan();
                byte[] bValue = RedisHelper.SerializeProtobuf(item);
                HashEntry entry = new HashEntry(hashKey, bValue);
                bool exists = await _cache.HashExistsAsync(key, hashKey);
                if (!exists)
                {
                    Task<bool> res = _cache.HashSetAsync(key, hashKey, bValue);
                    return res;
                }
                // bool res = azureCache.StringSet(key, CacheHelper.Serialize(item), TimeSpan.FromMinutes(EXPIRATION_MINUTS));
                totalTime = DateTime.Now - StartTime;
               // LogItem log = new LogItem() { LogType = LOGTYPE.Cache, Message = string.Format("INSERT ITEM KEY : {0}, RES : {1} TIME: {2}", key, res, totalTime) };
              //  LoggingFactory.GetRedisLogger.Custom(log);
            }
            catch (Exception ex)
            {
                // ILogger logger = LoggingFactory.GetRedisLogger;
                _logger.LogError("ERROR REDIS InsertHashAsync = {0}".StringFormat(ex.Message));
                
            }
            return Task.FromResult<bool>(false);

        }

        public async Task<bool> InsertHashJsonAsync(string key, string hashKey, object item, bool force = false)
        {
            try
            {
                var StartTime = DateTime.Now;
                var totalTime = new TimeSpan();

                string val = RedisHelper.SerializeJson(item);
                HashEntry entry = new HashEntry(hashKey, val);
                bool exists = await _cache.HashExistsAsync(key, hashKey);
                if (force || !exists)
                {
                    if (exists)
                        await RemoveHashKey(key, hashKey);
                    bool res = await _cache.HashSetAsync(key, hashKey, val);
                    _logger.LogInformation($"INSERT REDIS HashSetAsync HASH KEY = {hashKey}, VALUE = {val}");
                    return res;
                }
                // bool res = azureCache.StringSet(key, CacheHelper.Serialize(item), TimeSpan.FromMinutes(EXPIRATION_MINUTS));
                totalTime = DateTime.Now - StartTime;
                // LogItem log = new LogItem() { LogType = LOGTYPE.Cache, Message = string.Format("INSERT ITEM KEY : {0}, RES : {1} TIME: {2}", key, res, totalTime) };
                //  LoggingFactory.GetRedisLogger.Custom(log);
            }
            catch (Exception ex)
            {
                // ILogger logger = LoggingFactory.GetRedisLogger;
                _logger.LogError("ERROR REDIS InsertHashAsync = {0}".StringFormat(ex.Message));

            }
            return false;

        }

        public async Task<Task<bool>> RemoveHashKey(string key, string hashKey)
        {
            try
            {
                var StartTime = DateTime.Now;
                var totalTime = new TimeSpan();
               
                bool exists = await _cache.HashExistsAsync(key, hashKey);
                if (exists)
                {
                    Task<bool> res = _cache.HashDeleteAsync(key, hashKey);
                    _logger.LogInformation($"DELETE REDIS HashDeleteAsync HASH KEY = {hashKey}");
                    return res;
                }
                totalTime = DateTime.Now - StartTime;
               
            }
            catch (Exception ex)
            {
                // ILogger logger = LoggingFactory.GetRedisLogger;
                _logger.LogError("ERROR REDIS HashDeleteAsync = {0}".StringFormat(ex.Message));

            }
            return Task.FromResult<bool>(false);

        }

        public async Task<TResult> GetHashValueAsync<TResult>(string key, string hashKey) 
        {
            try
            {
                var StartTime = DateTime.Now;
               // var totalTime = new TimeSpan();
                RedisValue rVal = await _cache.HashGetAsync(key, hashKey);
                var res = RedisHelper.DeserializeProtobuf<TResult>(rVal);
                return res;
            }
            catch (Exception ex)
            {
                // ILogger logger = LoggingFactory.GetRedisLogger;
                _logger.LogError("ERROR REDIS GetHashValueAsync = {0}".StringFormat(ex.Message));
            }
            return default(TResult);

        }

        public async Task<TResult> GetHashValueJsonAsync<TResult>(string key, string hashKey)
        {
            try
            {
                var StartTime = DateTime.Now;
               // var totalTime = new TimeSpan();
                RedisValue rVal = await _cache.HashGetAsync(key, hashKey);
                var res = RedisHelper.DeserializeJson<TResult>(rVal);
                return res;
            }
            catch (Exception ex)
            {
                // ILogger logger = LoggingFactory.GetRedisLogger;
                _logger.LogError("ERROR REDIS GetHashValueAsync = {0}".StringFormat(ex.Message));
            }
            return default(TResult);

        }

        public async Task<List<TResult>> GetHashListValueJsonAsync<TResult>(string key)
        {
            try
            {
                var StartTime = DateTime.Now;
              //  var totalTime = new TimeSpan();
                HashEntry[] rVal = await _cache.HashGetAllAsync(key);
                List<TResult> res = new List<TResult>();
                foreach (var item in rVal)
                {
                    var rs = RedisHelper.DeserializeJson<TResult>(item.Value);
                    res.Add(rs);
                }
                
                return res;
            }
            catch (Exception ex)
            {
                // ILogger logger = LoggingFactory.GetRedisLogger;
                _logger.LogError("ERROR REDIS GetHashValueAsync = {0}".StringFormat(ex.Message));
            }
            return default(List<TResult>);

        }

        public bool InsertHash(string key, string hashKey, object item)
        {
            try
            {
                var StartTime = DateTime.Now;
               // var totalTime = new TimeSpan();
                byte[] bValue = RedisHelper.SerializeProtobuf(item);
                HashEntry entry = new HashEntry(hashKey, bValue);
                bool res = _cache.HashSet(key, hashKey, bValue);
                return res;
                // bool res = azureCache.StringSet(key, CacheHelper.Serialize(item), TimeSpan.FromMinutes(EXPIRATION_MINUTS));
                //totalTime = DateTime.Now - StartTime;
                // LogItem log = new LogItem() { LogType = LOGTYPE.Cache, Message = string.Format("INSERT ITEM KEY : {0}, RES : {1} TIME: {2}", key, res, totalTime) };
                //  LoggingFactory.GetRedisLogger.Custom(log);
            }
            catch (Exception ex)
            {
                // ILogger logger = LoggingFactory.GetRedisLogger;
                _logger.LogError("ERROR REDIS InsertHash = {0}".StringFormat(ex.Message));
                return false;
            }

        }
    }
}
