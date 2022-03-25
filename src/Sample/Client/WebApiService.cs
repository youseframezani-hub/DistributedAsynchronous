using DistributedAsync.Abstractions;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class WebApiService
    {
        private readonly IChannelFactory _channelFactory;

        public WebApiService(IChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
        }

        public async Task<ResultValue> GetValueAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var channelName = id.ToString();
            using var channelReader = await _channelFactory.CreateChannelReaderAsync<ResultValue>(channelName);
            
            var response = await $"https://localhost:44351/api/Values/{id}".GetAsync(cancellationToken);
            if (response.StatusCode != 202)
                throw new Exception("status code is not 202");

            Console.WriteLine($"get response 202 and wait for result... , id:{id}");
            return await channelReader.ReadAsync(cancellationToken);
        }
    }

    class ResultValue
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
