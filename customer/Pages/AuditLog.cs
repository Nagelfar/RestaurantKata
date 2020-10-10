using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Api.GuestExperience.Model;
using Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using Api.TableService.Model;
using System.Collections.Immutable;
using Newtonsoft.Json;

namespace Customer.Pages
{

    public class AuditLogModel : PageModel
    {
        private readonly ILogger<AuditLogModel> _logger;
        private readonly EventStore _events;

        public ImmutableList<LogEntry> Log { get; }

        public class LogEntry
        {
            public int? Guest { get; private set; }
            public string EventName { get; private set; }
            public DateTime On { get; private set; }
            public string Payload { get; private set; }

            internal static LogEntry FromEvent(Event @event)
            {
                var guest = (@event as IGuestEvent)?.Guest;
                return new LogEntry
                {
                    Guest = guest,
                    EventName = @event.GetType().Name,
                    On = @event.On,
                    Payload = JsonConvert.SerializeObject(@event, Formatting.Indented)
                };
            }
        }

        public AuditLogModel(ILogger<AuditLogModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _events = events;
            Log = events.Project(
                ImmutableList<LogEntry>.Empty,
                (state, @event) => state.Add(LogEntry.FromEvent(@event))
            );
        }


    }
}
