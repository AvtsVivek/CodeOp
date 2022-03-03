using System;
using System.Linq;
using Shouldly;

namespace EventSourcing.Demo
{
    public abstract class AggregateTests<TAggregate> where TAggregate : AggregateRoot
    {
        private readonly TAggregate _aggregateRoot;

        protected AggregateTests(TAggregate aggregateRoot)
        {
            _aggregateRoot = aggregateRoot;
        }

        protected void Given(params IEvent[] events)
        {
            if (events != null)
            {
                _aggregateRoot.Load(events);
            }
        }

        protected void When(Action<TAggregate> command)
        {
            command(_aggregateRoot);
        }

        protected void Then<TEvent>(params Action<TEvent>[] conditions)
        {
            var events = _aggregateRoot.GetUncommittedEvents();
            events.Count.ShouldBe(1);
            var evnt = events.First();
            evnt.ShouldBeOfType<TEvent>();
            if (conditions != null)
            {
                ((TEvent)evnt).ShouldSatisfyAllConditions(conditions);
            }
        }

        protected void Throws<TException>(Action<TAggregate> command, params Action<TException>[] conditions) where TException : Exception
        {
            var ex = Should.Throw<TException>(() => command(_aggregateRoot));
            if (conditions != null)
            {
                ex.ShouldSatisfyAllConditions(conditions);
            }
        }
    }
}