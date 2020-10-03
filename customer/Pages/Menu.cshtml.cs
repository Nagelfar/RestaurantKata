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

            Menus = _events.Project(
                ImmutableDictionary<int, Menu>.Empty,
                (state, @event) => @event switch
                {
                    MenuRetrieved menu => state.Add(menu.Menu.Guest, menu.Menu),
                    _ => state
                });
        }

        private IReadOnlyDictionary<int, Menu> Menus { get; set; }
        public Menu Menu { get; private set; }

        [BindProperty]
        public OrderCommand Order { get; set; }

        public void OnGet(int guest)
        {
            if (!Menus.ContainsKey(guest))
                RedirectToAction("/Index");

            Menu = Menus[guest];

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

            var menu = Menus[Order.Guest];

            _events.Append(new OrderPlaced
            {
                Guest = order.Guest,
                FoodOrder = order.Food.Select(f => menu.Food.Select(Convert).Single(x => x.Id == f)).ToList(),
                DrinkOrder = order.Drinks.Select(d => Menus[Order.Guest].Drinks.Select(Convert).Single(x => x.Id == d)).ToList(),
                Confirmation = confirmation
            });


            return RedirectToPage("Orders/Detail", new { guest = order.Guest });
        }
    }
}
