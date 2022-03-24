using DistributedAsync.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DistributedAsync.Redis
{
    public static class RedisServiceCollectionExtensions
    {

        /// <summary>
        /// Add redis channel.
        /// </summary>
        /// <returns>The redis channel.</returns>
        /// <param name="services">Services.</param>
        /// <param name="setupAction">Setup action.</param>
        public static IServiceCollection AddRedisChannel(this IServiceCollection services, Action<RedisOptions> setupAction)
        {
            var redisOptions = new RedisOptions();
            setupAction(redisOptions);
            services.AddSingleton<IChannelFactory>(new RedisChannelFactory(redisOptions.ConnectionString));

            return services;
        }
    }

    public class RedisOptions
    {
        public string ConnectionString { get; set; }
    }
}
