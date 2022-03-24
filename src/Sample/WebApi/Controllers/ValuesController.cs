using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DistributedAsync.Abstractions;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IChannelFactory _channelFactory;
        public ValuesController(IChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            Task.Run(() => GetResultByDemon(id));
            var result = new ObjectResult(null);
            result.StatusCode = 202;
            return result;
        }
        private void GetResultByDemon(Guid id)
        {
            var channelName = id.ToString();
            Thread.Sleep(5000);
            var result = new ResultValue { Id = id, Name = "Test value" };
            var channelWriter = _channelFactory.CreateChannelWriter<ResultValue>(channelName);
            channelWriter.Write(result);
        }

    }

    class ResultValue
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
