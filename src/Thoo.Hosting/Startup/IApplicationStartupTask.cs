namespace Thoo.Hosting.Startup;

public interface IApplicationStartupTask
{
    Task ExecuteAsync(CancellationToken ct);
}