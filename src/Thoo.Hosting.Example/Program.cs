using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Thoo.Hosting.Example.Services;

namespace Thoo.Hosting.Example;

public static class Program
{

    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            
        var host = CreateHost(configuration);
        host.Run();
    }

    private static IHost CreateHost(IConfiguration configuration)
        => Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddInstallers(configuration);
                services.AddStartupService();

                services.AddHostedService<TestService>();
            })
            .Build();

}