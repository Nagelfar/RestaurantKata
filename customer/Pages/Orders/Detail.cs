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

namespace Customer.Pages.Orders
{
    using GuestOrders = ImmutableDictionary<int, ImmutableDictionary<int, OrderViewModel>>;

    public class DetailModel : PageModel
    {
        private readonly ILogger<DetailModel> _logger;
        private readonly EventStore _events;

        public DetailModel(ILogger<DetailModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _events = events;
        }

        public void OnGet(int guest)
        {
            Orders = _events
                .IncludeOnly(@event =>
                    @event switch
                    {
                        OrderPlaced order => order.Guest == guest,
                        DeliveryReceived delivery => delivery.Guest == guest,
                        _ => false
                    }
                )
                .Project(GuestOrders.Empty, OrderViewModel.Projection)
                .Values.SelectMany(x => x.Values)
                .ToList();
            Guest = guest;
        }

        public IEnumerable<OrderViewModel> Orders { get; private set; }
        public int Guest { get; private set; }
    }


}
