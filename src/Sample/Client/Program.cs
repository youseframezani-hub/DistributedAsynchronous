using DistributedAsync.Redis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static IServiceProvider serviceProvider;
        static async Task Main(string[] args)
        {
            serviceProvider = SetupDependencyInjection();
            for (int i = 0; i < 8; i++)
            {
                Task.Run(async ()=> await CallApi()); 
            }

            Console.ReadLine();
        }

        private static async Task CallApi()
        {
            var webApi = serviceProvider.GetService<WebApiService>();
            var id = Guid.NewGuid();
            try
            {

                Console.WriteLine($"start call api with id:{id}");
                var result = await webApi.GetValueAsync(id, new CancellationTokenSource(16000).Token);
                Console.WriteLine($"result api, id:{result.Id}, name:{result.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed call api with id:{id}, message:{ex.Message}");
            }
        }


        static IServiceProvider SetupDependencyInjection() => new ServiceCollection()
            .AddRedisChannel(option => option.ConnectionString = "localhost")
            .AddSingleton<WebApiService>().BuildServiceProvider();

    }
}
