using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace LockAndWait
{
    public static class LockAndWaitServiceCollectionExtensions
    {
        public static IServiceCollection AddLockAndWaitService(this IServiceCollection services, string redisConnectionString, int database = -1)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (string.IsNullOrWhiteSpace(redisConnectionString))
                throw new ArgumentException("Redis connection string is required.", nameof(redisConnectionString));

            services.TryAddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));
            services.TryAddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase(database));
            services.TryAddSingleton<ILockAndWaitService, LockAndWaitService>();
            return services;
        }
    }
}
