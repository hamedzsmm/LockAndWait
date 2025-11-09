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

        await lockAndWaitService.AcquireAsync(Key, TimeSpan.FromSeconds(30));


        //In Another Command Or Application Can Wait For This Lock
        await DoAnything(sp);
    }

    static async Task DoAnything(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var lockAndWaitService = scope.ServiceProvider.GetRequiredService<ILockAndWaitService>();

        Console.WriteLine("Waiting ...");
        var lockIsFree = await lockAndWaitService.WaitAsync(Key, timeout: TimeSpan.FromSeconds(5));

        Console.WriteLine(lockIsFree ? "Lock is free , DoAnything Is Running" : "Timeout on Waiting for lock was timeout");
    }
}