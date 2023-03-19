
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;
using System;

namespace PublisherTest // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        private const string RedisConnectionString = "localhost:6379";
        private static IConnectionMultiplexer connection;
        private const string Channel = "test-channel";
        static void Main(string[] args)
        {
            Init();
            RedisKeyspaceNotifications.NotificationsExample(connection);
            //RunSub();
            Console.ReadLine();
        }

        private static void RunSub()
        {
            var sub = connection.GetSubscriber();
           

            for (int i = 0; i <= 10; i++)
            {
                sub.Publish(Channel, $"Message {i}");
                Console.WriteLine($"Message {i} published successfully");
                Thread.Sleep(2000);
            }

            //pubsub.PublishAsync(Channel, "This is a test message!!", CommandFlags.FireAndForget);
            Console.Write("Message Successfully sent to test-channel");
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

