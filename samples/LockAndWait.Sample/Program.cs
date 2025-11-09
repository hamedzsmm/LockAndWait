using Microsoft.Extensions.DependencyInjection;

namespace LockAndWait.Sample;

internal class Program
{
    private static readonly string Key = "DemoLock";

    private static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddLockAndWaitService("localhost:6379");

        await using var sp = services.BuildServiceProvider();
        var lockAndWaitService = sp.GetRequiredService<ILockAndWaitService>();

        lockAndWaitService.AcquireAsync(Key, TimeSpan.FromSeconds(30)).GetAwaiter().GetResult();


        //In Another Command Or Application Can Wait For This Lock
        await DoAnything(sp);
    }

    static Task DoAnything(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var lockAndWaitService = scope.ServiceProvider.GetRequiredService<ILockAndWaitService>();

        Console.WriteLine("Waiting ...");
        var lockIsFree = lockAndWaitService.WaitAsync(Key, timeout: TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

        Console.WriteLine(lockIsFree ? "Lock is free , DoAnything Is Running" : "Timeout on Waiting for lock was timeout");
        return Task.CompletedTask;
    }
}