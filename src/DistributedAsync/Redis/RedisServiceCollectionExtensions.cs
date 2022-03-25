using DistributedAsync.Abstractions;
using DistributedAsync.Tools;
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
            services.AddSingleton<ISerializer, NewtonsoftSerializer>();
            services.AddSingleton<IChannelFactory>(s => new RedisChannelFactory(redisOptions, s.GetService<ISerializer>()));

            return services;
        }
    }

    public class RedisOptions
    {
        public string ConnectionString { get; set; }
        public bool IsPersistence { get; set; }
        public PersistenceOption PersistenceOption { get; set; }
    }
    public class PersistenceOption
    {
        public int DataBaseNumber { get; set; }
        public int PersistenceExpirationInMinute { get; set; }
        internal TimeSpan PersistenceExpiration => TimeSpan.FromMinutes(PersistenceExpirationInMinute);
    }
}
