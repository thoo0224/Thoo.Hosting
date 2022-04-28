using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Thoo.Hosting.Startup;

namespace Thoo.Hosting.Example.Tasks;

public class TestStartupTask : IApplicationStartupTask
{

    private readonly ILogger<TestStartupTask> _logger;

    public TestStartupTask(ILogger<TestStartupTask> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Running startup task..");
        await Task.Delay(1000, ct);
        
        _logger.LogInformation("Finished");
    }
    
}