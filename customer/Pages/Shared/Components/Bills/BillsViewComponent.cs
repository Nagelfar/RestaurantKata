using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Pages.Billing.Components.Bills
{
    public class BillsViewComponent : ViewComponent
    {
        private readonly EventStore events;

        public BillsViewComponent(EventStore events)
        {
            this.events = events;
        }

        public IViewComponentResult Invoke(int guest)
        {
            var bills = events
               .Project(
                   ImmutableDictionary<int, BillViewModel>.Empty,
                   BillViewModel.BillsFor(guest)
               );
            return View(bills.Values.ToImmutableList());
        }

    }
}