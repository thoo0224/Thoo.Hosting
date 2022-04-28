using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Thoo.Hosting;

public interface IServiceInstaller
{
    void Configure(IServiceCollection services, IConfiguration configuration);
}