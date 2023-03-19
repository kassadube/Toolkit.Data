using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisQueueStack;
public class RedisService
{
}

public class RedisData
{
    IConnectionMultiplexer _connectionMultiplexer;
    public IDatabase _cache = null;
    ILogger _logger;
    public RedisData(IConnectionMultiplexer connectionMultiplexer, ILogger logger)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
        ConnectDB();
    }

    private void ConnectDB()
    {
        try
        {
             _cache = _connectionMultiplexer.GetDatabase();
           

        }
        catch (Exception ex)
        {
            // ILogger logger = LoggingFactory.GetLogger;
            _logger.LogError($"ERROR CONNECTING TO REDIS MESSAGE = {ex.Message}");
            throw;
        }
    }

}

public class RedisStack: RedisData
{
    
    public RedisStack(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisService> logger) : base(connectionMultiplexer, logger)
    {

    }
    public  void Push(RedisKey stackName, RedisValue value)
    {
        _cache.ListRightPush(stackName, value);
    }

    public  RedisValue Pop(RedisKey stackName)
    {
        return _cache.ListRightPop(stackName);
    }
}

public class RedisQueue : RedisData
{
    public RedisQueue(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisService> logger) : base(connectionMultiplexer, logger)
    {

    }
    public  void Push(RedisKey queueName, RedisValue value)
    {
        _cache.ListRightPush(queueName, value);
    }

    public RedisValue Pop(RedisKey queueName)
    {
        return _cache.ListLeftPop(queueName);
    }
}
