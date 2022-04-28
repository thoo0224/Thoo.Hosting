using Microsoft.Extensions.Hosting;

namespace Thoo.Hosting.Startup;

public abstract class ApplicationService : BackgroundService
{

    private readonly ApplicationStartupService _startupService;

    protected ApplicationService(ApplicationStartupService startupService)
    {
        _startupService = startupService;
    }

    protected sealed override async Task ExecuteAsync(CancellationToken serviceCt)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(_startupService.Cts!.Token, serviceCt);
        var ct = cts.Token;

        await _startupService.WaitForStartupTasksAsync(ct).ConfigureAwait(false);
        await StartAsync(ct);
    }

    protected new abstract Task StartAsync(CancellationToken ct = default);
    protected new Task StopAsync(CancellationToken ct = default) => Task.CompletedTask;

}