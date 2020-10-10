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
using Customer.Pages.Billing;

namespace Customer.Pages.Orders
{
    using GuestOrders = ImmutableDictionary<int, ImmutableDictionary<int, OrderViewModel>>;

    public class DetailModel : PageModel
    {
        private readonly ILogger<DetailModel> _logger;
        private readonly EventStore _events;
        private readonly HttpClient _billingClient;

        public IEnumerable<OrderViewModel> Orders { get; private set; }
        public BillViewModel Bill { get; private set; }
        public int Guest { get; private set; }

        public DetailModel(ILogger<DetailModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _events = events;
            _billingClient = factory.CreateClient("Billing");
        }

        public void OnGet(int guest)
        {
            Orders = _events
                .IncludeOnly(@event =>
                    @event switch
                    {
                        OrderPlaced order => order.Guest == guest,
                        DeliveryReceived delivery => delivery.Guest == guest,
                        BillPaid paid => paid.Guest == guest,
                        _ => false
                    }
                )
                .Project(GuestOrders.Empty, OrderViewModel.Projection)
                .Values.SelectMany(x => x.Values)
                .ToList();
            Bill = _events.Project(default(BillViewModel),BillViewModel.OpenBillFor(guest));
            Guest = guest;
        }

        public async Task<IActionResult> OnPostRequestBillAsync(int guest)
        {
            var billContent = await _billingClient.PostAsync($"bills/{guest}", new StringContent(""));

            billContent.EnsureSuccessStatusCode();

            var bill = await billContent.Content.ReadContentAsJson<Api.Billing.Model.Bill>();

            if(billContent.StatusCode == System.Net.HttpStatusCode.OK)
                _events.Append(new BillReceived(guest, bill._Bill, bill.OrderedFood, bill.OrderedDrinks, bill.TotalSum));
            else 
                _events.Append(new BillUpdated(guest, bill._Bill, bill.OrderedFood, bill.OrderedDrinks, bill.TotalSum));

            return RedirectToPage("/Orders/Detail", new { guest = guest });
        }

    }


}
