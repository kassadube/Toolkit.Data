
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;
using System;

namespace SubscriberTestTest // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        private const string RedisConnectionString = "localhost:6379";
        private static IConnectionMultiplexer connection;
        private const string Channel = "test-channel";
        static void Main(string[] args)
        {
            Init();
            Console.WriteLine("Listening test-channel");
            var pubsub = connection.GetSubscriber();

            pubsub.Subscribe(Channel, (channel, message) => Console.Write("Message received from test-channel : " + message));
            Console.ReadLine();
        }
        private static IConfiguration _config;
        private static IServiceCollection _serviceCollection;
        public static IServiceProvider _serviceProvider;
        public static void Init()
        {
            _config = ConfigurationHelper.LoadAppConfiguration();
            ConfigurationHelper.ConfigLog(_config);
            Log.Information("INNT LOG");
            _serviceCollection = ConfigureDataServices.GetServiceConfiguration(_config);
            ConfigurationHelper.ConfigureServices(_serviceCollection);
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            Log.Information("INNT SERVICES");
            //ConnectionMultiplexer.Connect(RedisConnectionString);
            connection = _serviceProvider.GetService<IConnectionMultiplexer>();
        }

    }
}

