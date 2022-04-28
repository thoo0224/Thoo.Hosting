using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thoo.Hosting.Example.Services;

namespace Thoo.Hosting.Example.Installers;

public class TestInstaller : IServiceInstaller
{
    
    public void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<TestService>();
    }
    
}