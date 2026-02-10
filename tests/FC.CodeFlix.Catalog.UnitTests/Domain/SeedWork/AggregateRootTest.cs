using FluentAssertions;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.SeedWork
{
    public class AggregateRootTest
    {
        [Fact(DisplayName = nameof(RaiseEvent))]
        [Trait("Domain", "Aggregate Root")]
        public void RaiseEvent()
        {
            var domainEvent = new DomainEventFake();
            var aggregate = new AggregateRootFake();

            aggregate.RaiseEvent(domainEvent);

            aggregate.Events.Should().HaveCount(1);
        }

        [Fact(DisplayName = nameof(ClearEvents))]
        [Trait("Domain", "Aggregate Root")]
        public void ClearEvents()
        {
            var domainEvent = new DomainEventFake();
            var aggregate = new AggregateRootFake();
            aggregate.RaiseEvent(domainEvent);

            aggregate.ClearEvent();

            aggregate.Events.Should().HaveCount(0);
        }
    }
}
