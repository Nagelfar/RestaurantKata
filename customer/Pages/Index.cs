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

namespace Customer.Pages
{

    public class IndexModel : PageModel
    {
        private readonly ILogger<MenuModel> _logger;
        private readonly HttpClient _guestExperienceClient;
        private readonly EventStore _events;

        public class GuestViewModel
        {
            public int Guest { get; internal set; }
            public DateTime GuestSince { get; internal set; }
            public int Orders { get; internal set; }

            public GuestViewModel AddOrder() {
                Orders++;
                return this;
            }
        }

        private readonly ImmutableDictionary<int,GuestViewModel> _guests;
        public IEnumerable<GuestViewModel> Guests { get => _guests.Values; }

        public IndexModel(ILogger<MenuModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _guestExperienceClient = factory.CreateClient("GuestExperience");
            _events = events;

            _guests = _events.Project(
                ImmutableDictionary<int,GuestViewModel>.Empty,
                (state, @event) => @event switch
                {
                    MenuRetrieved menu => state.Add(
                        menu.Menu.Guest,
                        new GuestViewModel
                        {
                            Guest = menu.Menu.Guest,
                            GuestSince = menu.On,
                            Orders = 0
                        }
                    ),
                    OrderPlaced order => state.SetItem(
                        order.Guest,
                        state.GetValueOrDefault(order.Guest).AddOrder()
                    ),
                    _ => state
                });
        }

        public async Task<IActionResult> OnPostRetrieveMenuAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var menu = await _guestExperienceClient.GetFromJsonAsync<Menu>("menu");

            _events.Append(new MenuRetrieved { Menu = menu });

            return RedirectToPage("/Orders/Detail",new { guest = menu.Guest});
        }

    }
}
