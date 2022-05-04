using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Thoo.Hosting.Startup;

public class ApplicationStartupService : BackgroundService
{

    public CancellationTokenSource? Cts => _cts;

    private readonly ILogger<ApplicationStartupService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly ApplicationStartupOptions _options;
    private CancellationTokenSource? _cts;

    public ApplicationStartupService(
        ILogger<ApplicationStartupService> logger, IServiceProvider serviceProvider, ApplicationStartupOptions options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        
        var assembly = _options.Assembly ?? Assembly.GetEntryAssembly()!;
        var tasks = assembly.ExportedTypes
            .Where(x => x.IsAssignableTo(typeof(IApplicationStartupTask)) && !x.IsInterface)
            .Select(x => ActivatorUtilities.CreateInstance(_serviceProvider, x))
            .Cast<IApplicationStartupTask>()
            .ToArray();

        var taskCancellationToken = _cts.Token;
        if (_options.RunStartupTasksInParallel)
        {
            _logger.LogDebug("Running {NumTasks} startup tasks in parallel", tasks.Length);
            await Parallel.ForEachAsync(tasks, taskCancellationToken, async (task, taskCt) =>
                await ExecuteStartupTaskAsync(task, taskCt)).ConfigureAwait(false);
        }
        else
        {
            _logger.LogDebug("Running {NumTasks} startup tasks sequentially", tasks.Length);
            foreach (var task in tasks)
                await ExecuteStartupTaskAsync(task, taskCancellationToken).ConfigureAwait(false);
        }

        _cts.Cancel();
    }

    public async Task WaitForStartupTasksAsync(CancellationToken ct = default)
    {
        if (_cts is null)
            throw new NullReferenceException($"{nameof(_cts)} is not initialized yet.");

        var cancellationCallbackRegistration = ct.Register(_cts.Cancel);
        try
        {
            await Task.Delay(Timeout.Infinite, ct).ConfigureAwait(false);
        }
        catch
        {
            // ignored
        }
        finally
        {
             cancellationCallbackRegistration.Unregister();
        }
    }

    private async Task ExecuteStartupTaskAsync(IApplicationStartupTask task, CancellationToken ct)
    {
        try
        {
            await task.ExecuteAsync(ct);
        }
        catch
        {
            if (_options.ThrowOnException)
                throw;
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _cts?.Dispose();
    }
    
}