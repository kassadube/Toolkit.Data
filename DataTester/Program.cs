// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

Console.WriteLine("Hello, World!");
DataTesterRun.Init();
Console.ReadKey();
class DataTesterRun
{ 



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
    }
}
