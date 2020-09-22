using System;
using System.Collections.Generic;
using System.Linq;

namespace Customer
{
    public class EventStore
    {

        private List<Event> _events = new List<Event>();
        public void Append(Event @event)
        {
            _events.Add(@event);
        }

        public TState Project<TState>(TState initial, Func<TState, Event, TState> projector)
        {
            return _events.Aggregate(initial, projector);
        }
    }
}