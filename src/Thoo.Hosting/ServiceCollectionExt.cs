using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Thoo.Hosting.Startup;

namespace Thoo.Hosting;

public static class ServiceCollectionExt
{

    public static IServiceCollection AddStartupService(
        this IServiceCollection services, Action<ApplicationStartupOptions>?configureOptions = null)
    {
        var options = new ApplicationStartupOptions();
        configureOptions?.Invoke(options);
        
        return services
            .AddSingleton(options)
            .AddSingleton<ApplicationStartupService>()
            .AddSingleton<IHostedService, ApplicationStartupService>(
                provider => provider.GetRequiredService<ApplicationStartupService>());
    }

    public static IServiceCollection AddInstallers(
        this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly()!;

        var exportedTypes = assembly.GetExportedTypes();
        var installers = exportedTypes
            .Where(x => x.IsAssignableTo(typeof(IServiceInstaller)) && !x.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>()
            .ToArray();

        foreach(var installer in installers)
            installer.Configure(services, configuration);

        return services;
    }

}