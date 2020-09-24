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


namespace Customer.Pages
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }

    public class Order
    {
        public int Guest { get; set; }
        public OrderItem[] Food { get; set; }
        public OrderItem[] Drinks { get; set; }
    }

    public class MenuModel : PageModel
    {
        private readonly ILogger<MenuModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly EventStore _events;

        public MenuModel(ILogger<MenuModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _httpClient = factory.CreateClient("GuestExperience");
            _events = events;
        }

        public Menu Menu { get; set; }

        [BindProperty]
        public Order Order { get; set; }

        public void OnGet()
        {
            Menu = _events.Project(default(Menu), (state, @event) => @event switch
            {
                MenuRetrieved menu => menu.Menu,
                _ => state
            });
            if (Menu != null)
                Order = new Order { Guest = Menu.Guest };
        }


        public async Task<IActionResult> OnPostRetrieveMenuAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var menu = await _httpClient.GetFromJsonAsync<Menu>("menu");

            _events.Append(new MenuRetrieved { Menu = menu });

            return RedirectToPage("/Menu");
        }

        public async Task<IActionResult> OnPostOrderAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            



            return RedirectToPage("/Index");
        }
    }
}
