using System;
using Microsoft.Extensions.DependencyInjection;
using LockAndWait;

class Program
{
    static void Main()
    {
        var services = new ServiceCollection();
        services.AddLockAndWaitService("localhost:6379");

        using var sp = services.BuildServiceProvider();
        var svc = sp.GetRequiredService<ILockAndWaitService>();

        var key = "demo:lock";
        var handle = svc.AcquireAsync(key, TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        if (handle is null)
        {
            Console.WriteLine("Busy. Waiting ...");
            svc.WaitAsync(key, timeout: TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
            handle = svc.AcquireAsync(key, TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        }

        if (handle is not null)
        {
            Console.WriteLine($"Acquired: {handle}");
            System.Threading.Thread.Sleep(1000);
            var released = svc.ReleaseAsync(handle).GetAwaiter().GetResult();
            Console.WriteLine($"Released: {released}");
        }
        else
        {
            Console.WriteLine("Could not acquire the lock.");
        }
    }
}
