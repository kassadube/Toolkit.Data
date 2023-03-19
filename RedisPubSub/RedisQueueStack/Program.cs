
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;
using System;
using Toolkit.Configuration.Helper;

namespace RedisQueueStack // Note: actual namespace depends on the project name.
{
    internal class Program
    {
         private static IConnectionMultiplexer connection;
        private const string Channel = "test-channel";
        static RedisQueue rQueue;
        static void Main(string[] args)
        {
            Init();
            rQueue = _serviceProvider.GetRequiredService<RedisQueue>();
            rQueue.Push("TESTQ", "VT");

            var res = rQueue.Pop("TESTQ");
            //RunSub();
            Lazy<IConnectionMultiplexer> connectionMultiplexer = new Lazy<IConnectionMultiplexer>(connection);
            RedisJobQueue jQue = new RedisJobQueue(connectionMultiplexer, "TESTJOB");
            var s = jQue.AsManager();

            var consumer = jQue.AsConsumer();
            s.AddJob("asrtdasd");
            consumer.OnJobReceived += Consumer_OnJobReceived;
               

            Console.ReadLine();
        }

        private static void Consumer_OnJobReceived(object? sender, JobReceivedEventArgs e)
        {
           
        }

        private static IConfiguration _config;
        private static IServiceCollection _serviceCollection;
        public static IServiceProvider _serviceProvider;
        public static void Init()
        {
            _config = ConfigurationHelper.LoadAppConfiguration();
            ConfigurationHelper.ConfigLog(_config);
            Log.Information("INNT LOG");
            _serviceCollection = GetServiceConfiguration(_config);
            ConfigurationHelper.ConfigureServices(_serviceCollection);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            Log.Information("INNT SERVICES");
            //ConnectionMultiplexer.Connect(RedisConnectionString);
            connection = _serviceProvider.GetService<IConnectionMultiplexer>();
        }

        public static IServiceCollection GetServiceConfiguration(IConfiguration config)
        {

            var collection = new ServiceCollection()
                // .AddTransient<TestRepository, TestRepository>()               
                //  .AddSingleton<RedisService, RedisService>()               
                //  .AddSingleton<RedisTester, RedisTester>()               
                //  .AddDistributedRedisCache(options => options.Configuration = config["ConnectionStrings:RedisConnection"])
                .AddTransient<RedisQueue, RedisQueue>()
                .AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(config["ConnectionStrings:RedisConnection"]))
                .AddSingleton(config);
            collection.BuildServiceProvider();
            return collection;
        }
    }
}

