using System;
using DataTester.repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;


    public class ConfigureDataServices
    {
        public static IServiceCollection GetServiceConfiguration(IConfiguration config)
        {

            var collection = new ServiceCollection()
                .AddTransient<TestRepository, TestRepository>()               
                .AddDistributedRedisCache(options => options.Configuration = config["ConnectionStrings:RedisConnection"])
                .AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(config["ConnectionStrings:RedisConnection"]))                
                .AddSingleton(config);
            collection.BuildServiceProvider();
            return collection;
        }
       
    }

