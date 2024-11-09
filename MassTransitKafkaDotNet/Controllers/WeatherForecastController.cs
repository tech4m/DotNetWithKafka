using Microsoft.AspNetCore.Mvc;
using MassTransit.KafkaIntegration;
using MassTransit;
using MassTransitKafkaDotNet.Model;
using System.Net;

namespace MassTransitKafkaDotNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ITopicProducer<VideoCreatedEvent> _topicProducer;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            ITopicProducer<VideoCreatedEvent> topicProducer)
        {
            _logger = logger;
            this._topicProducer = topicProducer;
        }


        [HttpPost("{title}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostAsync(string title)
        {
            await _topicProducer.Produce(new VideoCreatedEvent
            {
                Title = $"{title}"
            });

            return Ok(title);
        }
    }
}
