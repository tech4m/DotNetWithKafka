using MassTransit;

namespace MassTransitKafkaDotNet.Model;

public class VideoDeletedEventConsumer : IConsumer<VideoDeletedEvent>
{
    public Task Consume(ConsumeContext<VideoDeletedEvent> context)
    {
        var message = context.Message.Title;
        return Task.CompletedTask;
    }
}
