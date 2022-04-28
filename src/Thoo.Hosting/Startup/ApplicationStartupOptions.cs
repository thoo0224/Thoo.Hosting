using System.Reflection;

namespace Thoo.Hosting.Startup;

public class ApplicationStartupOptions
{
    public Assembly? Assembly { get; set; }
    public bool ThrowOnException { get; set; } = true;
    public bool RunStartupTasksInParallel { get; set; } = true;
}