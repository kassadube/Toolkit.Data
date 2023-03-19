using Microsoft.Extensions.Logging;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Data;

namespace DataTester
{
    public class RedisTester
    {
        RedisService _redisService;
        ILogger<RedisTester> _logger;
        string _key = "redisTester";

        public RedisTester(RedisService redisService, ILogger<RedisTester> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        public async Task<bool> Insert()
        {
            string key = "v1";
            var data = new { item = 1, val = "df" };
            var rRes = await _redisService.InsertHashJsonAsync(_key,key, data, true);
            return rRes;
        }
        public async Task<object> Read()
        {
            string key = "v1";
            var data = new { item = 1, val = "df" };
            var res = await _redisService.GetHashValueJsonAsync<dynamic>(_key, key);
            return res;
        }

    }
}
