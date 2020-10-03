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
        private readonly HttpClient _billingClient;
        private readonly EventStore _events;

        public IEnumerable<OrderViewModel> Orders { get; private set; }
        public BillViewModel Bill { get; private set; }
        public int Guest { get; private set; }

        public class BillViewModel
        {
            public int Guest { get; internal set; }
            public int BilledItems { get; internal set; }
            public decimal TotalSum { get; internal set; }
            public DateTime BillFrom { get; internal set; }
        }


        public DetailModel(ILogger<DetailModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _billingClient = factory.CreateClient("Billing");
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
            Bill = _events
                .Project(
                    default(BillViewModel),
                    (state, @events) => @events switch
                    {
                        BillReceived b when b.Guest == guest =>
                            new BillViewModel
                            {
                                Guest = guest,
                                BilledItems = b.OrderedFood.Concat(b.OrderedDrinks).Count(),
                                TotalSum = b.TotalSum,
                                BillFrom = b.On
                            },
                        _ => state
                    }
                );
            Guest = guest;
        }

        public async Task<IActionResult> OnPostRequestBillAsync(int guest)
        {
            var billContent = await _billingClient.PostAsync($"bills/{guest}", new StringContent(""));
            var bill = await billContent.Content.ReadContentAsJson<Api.Billing.Model.Bill>();

            _events.Append(new BillReceived(guest, bill._Bill, bill.OrderedFood, bill.OrderedDrinks, bill.TotalSum));

            return RedirectToPage("/Orders/Detail", new { guest = guest });
        }

    }


}
