// See https://aka.ms/new-console-template for more information
using DataTester;
using DataTester.repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Toolkit.Model;

Console.WriteLine("Hello, World!");
DataTesterRun.Init();
DataTesterRun.GetAccountUsers(4326);
DataTesterRun.GetAccountDetailes(4326);
DataTesterRun.TestRedis();

Console.ReadKey();

class DataTesterRun
{
    



    private static IConfiguration _config;
    private static IServiceCollection _serviceCollection;
    public static IServiceProvider _serviceProvider;
    public static RedisTester _redisTester;
    public static void Init()
    {
        _config = ConfigurationHelper.LoadAppConfiguration();
        ConfigurationHelper.ConfigLog(_config);
        Log.Information("INNT LOG");
        _serviceCollection = ConfigureDataServices.GetServiceConfiguration(_config);
        ConfigurationHelper.ConfigureServices(_serviceCollection);
        _serviceProvider = _serviceCollection.BuildServiceProvider();
        _redisTester = _serviceProvider.GetRequiredService<RedisTester>();
        Log.Information("INNT SERVICES");
    }

    public static void GetAccountUsers(int accountId)
    {
        var rep = _serviceProvider.GetRequiredService<TestRepository>();
        BaseREQ req = new BaseREQ() { AccountId = accountId };
        var res = rep.GetAccountUsers(req);
       
        Console.WriteLine(res.IsSucceded);
    }

    public static void GetAccountDetailes(int accountId)
    {
        var rep = _serviceProvider.GetRequiredService<TestRepository>();
        BaseREQ req = new BaseREQ() { AccountId = accountId };
        var res = rep.GetAccountDetails(req);

        Console.WriteLine(res.IsSucceded);
    }

    public static void TestRedis()
    {
        var x = Task.Run(async () => await _redisTester.Insert());
        Task.WaitAll(x);
        Console.WriteLine(x.Result);

        var res = Task.Run(async () => await _redisTester.Read());
        Task.WaitAll(res);
        Console.WriteLine(res.Result);
    }
}
