using System;
using System.Collections.Generic;
using System.Linq;

namespace Customer
{
    public delegate TState Projector<TState>(TState state, Event @event);

    public class EventStore
    {

        private List<Event> _events = new List<Event>();
        public void Append(Event @event)
        {
            lock (_events)
            {
                _events.Add(@event);
            }
        }

        public TState Project<TState>(TState initial, Projector<TState> projector)
        {
            return _events.ToList().Project(initial, projector);
        }
        public IReadOnlyCollection<Event> IncludeOnly(Func<Event, bool> predicate) => 
            _events
                .ToList()
                .Where(predicate)
                .ToList();
    }

    public static class ProjectionExtensions
    {
        public static TState Project<TState>(this IReadOnlyCollection<Event> messages, TState initial, Projector<TState> projector)
        {
            return messages.Aggregate(initial, projector.Invoke);
        }
    }
}