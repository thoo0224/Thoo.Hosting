using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Thoo.Hosting.Startup;

namespace Thoo.Hosting.Example.Services;

public class TestService : ApplicationService
{

    private readonly ILogger<TestService> _logger;

    public TestService(ILogger<TestService> logger, ApplicationStartupService startupService)
        : base(startupService)
    {
        _logger = logger;
    }
    
    protected override Task StartAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Executing test service");
        return Task.CompletedTask;
    }
    
}