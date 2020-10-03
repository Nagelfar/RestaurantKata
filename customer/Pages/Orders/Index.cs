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


    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly EventStore _events;
        private readonly GuestOrders _guests;


        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _events = events;

            _guests = _events.Project(GuestOrders.Empty, OrderViewModel.Projection);
        }

        public void OnGet()
        {

        }

        public IEnumerable<GuestOrderModel> Orders
        {
            get => _guests
                .Select(x => new GuestOrderModel { Guest = x.Key, Orders = x.Value.Values })
                .ToList();
        }

    }


}
