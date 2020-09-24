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

namespace Customer.Pages
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
    }

    public class OrderCommand
    {
        public int Guest { get; set; }
        public OrderItem[] Food { get; set; }
        public OrderItem[] Drinks { get; set; }
    }

    public class MenuModel : PageModel
    {
        private readonly ILogger<MenuModel> _logger;
        private readonly HttpClient _tableServiceClient;
        private readonly EventStore _events;

        public MenuModel(ILogger<MenuModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _tableServiceClient = factory.CreateClient("TableService");
            _events = events;

            Menu = _events.Project(default(Menu), (state, @event) => @event switch
            {
                MenuRetrieved menu => menu.Menu,
                _ => state
            });
        }

        public Menu Menu { get; set; }

        [BindProperty]
        public OrderCommand Order { get; set; }

        public void OnGet()
        {
            if(Menu == null)
                RedirectToAction("/Index");

            Order = new OrderCommand { Guest = Menu.Guest };
        }

        private OrderPlaced.OrderItem Convert(MenuItem item) => new OrderPlaced.OrderItem
        {
            Name = item.Name,
            Price = item.Price,
            Id = item.Id,
            Nutrition = item.Nutrition.Select(x => x.ToString()).ToList()
        };


        public async Task<IActionResult> OnPostOrderAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var order = new Order
            {
                Guest = Order.Guest,
                Food =
                    Order.Food
                        .Where(x => x.Quantity.HasValue && x.Quantity > 0)
                        .SelectMany(x => Enumerable.Repeat(x.Id, x.Quantity.Value))
                        .ToList(),
                Drinks =
                    Order.Drinks
                        .Where(x => x.Quantity.HasValue && x.Quantity > 0)
                        .SelectMany(x => Enumerable.Repeat(x.Id, x.Quantity.Value))
                        .ToList()
            };

            var response = await _tableServiceClient.PostJsonAsync("orders", order);

            response.EnsureSuccessStatusCode();

            var confirmation = await response.Content.ReadContentAsJson<OrderConfirmation>();

            _events.Append(new OrderPlaced
            {
                Guest = order.Guest,
                Order =
                    order.Food.Select(f => Menu.Food.Select(Convert).Single(x => x.Id == f))
                    .Concat(order.Drinks.Select(d => Menu.Drinks.Select(Convert).Single(x => x.Id == d)))
                    .ToList(),
                Confirmation = confirmation
            });


            return RedirectToPage("/Orders", new { order = confirmation.Order });
        }
    }
}
