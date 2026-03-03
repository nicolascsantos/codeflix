using FC.CodeFlix.Catalog.Application.EventHandlers;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Domain.Events;
using Moq;

namespace FC.CodeFlix.Catalog.UnitTests.Application.EventHandlers
{
    public class SendToEncoderEventHandlerTest
    {
        [Fact(DisplayName = nameof(HandleAsync))]
        [Trait("Application", "EventHandlers")]
        public async Task HandleAsync()
        {
            var messageProducerMock = new Mock<IMessageProducer>();
            messageProducerMock.Setup(x => x.SendMessageAsync(
                It.IsAny<VideoUploadedEvent>(),
                It.IsAny<CancellationToken>()
            )).Returns(Task.CompletedTask);
            var handler = new SendToEncoderEventHandler(messageProducerMock.Object);
            VideoUploadedEvent @event = new VideoUploadedEvent(Guid.NewGuid(), "medias/video.mp4");

            await handler.HandleAsync(@event, CancellationToken.None);

            messageProducerMock.Verify(x => x.SendMessageAsync(@event, CancellationToken.None), Times.Once);
        }
    }
}
