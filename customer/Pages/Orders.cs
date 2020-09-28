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

    public class OrderViewModel
    {
        public OrderViewModel(int guest, List<OrderItem> foodOrder, List<OrderItem> drinkOrder, OrderConfirmation confirmation)
        {
            Guest = guest;
            FoodOrder = foodOrder;
            DrinkOrder = drinkOrder;
            Confirmation = confirmation;
        }

        public int Guest { get; }
        public List<OrderItem> FoodOrder { get; }
        public List<OrderItem> DrinkOrder { get; }
        public OrderConfirmation Confirmation { get; }

        public class OrderItem
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public List<string> Nutrition { get; set; }

            public decimal Price { get; set; }
            public Delivery ReceivedAt { get; set; }

            public static OrderItem Create(OrderPlaced.OrderItem item) => new OrderItem
            {
                Id = item.Id,
                Name = item.Name,
                Nutrition = item.Nutrition,
                Price = item.Price,
                ReceivedAt = null
            };


            public class Delivery
            {
                public int DeliveryId { get; set; }
                public DateTime DeliveredOn { get; set; }
            }
        }
    }

    public class OrdersModel : PageModel
    {
        private readonly ILogger<MenuModel> _logger;
        private readonly EventStore _events;
        private readonly ImmutableDictionary<int, OrderViewModel> _orders;

        private static void MarkUndeliveredItem(IEnumerable<OrderViewModel.OrderItem> items, IEnumerable<int> deliveryItems, OrderViewModel.OrderItem.Delivery delivery)
        {
            foreach (var deliveredId in deliveryItems)
            {
                var undeliveredItem =
                    items
                        .Where(x => x.ReceivedAt == null)
                        .Where(x => x.Id == deliveredId)
                        .FirstOrDefault();
                if (undeliveredItem != null)
                    undeliveredItem.ReceivedAt = delivery;
            }
        }

        private static ImmutableDictionary<int, OrderViewModel> MarkAsDelivered(ImmutableDictionary<int, OrderViewModel> state, DeliveryReceived delivery)
        {
            if (state.TryGetValue(delivery.Order, out var model))
            {
                var deliveryMark = new OrderViewModel.OrderItem.Delivery
                {
                    DeliveredOn = delivery.On,
                    DeliveryId = delivery.Delivery
                };
                MarkUndeliveredItem(model.FoodOrder, delivery.Items.Food, deliveryMark);
                MarkUndeliveredItem(model.DrinkOrder, delivery.Items.Drinks, deliveryMark);
            }
            return state;
        }

        public OrdersModel(ILogger<MenuModel> logger, IHttpClientFactory factory, EventStore events)
        {
            _logger = logger;
            _events = events;

            _orders = _events.Project(ImmutableDictionary<int, OrderViewModel>.Empty, (state, @event) => @event switch
             {
                 OrderPlaced order => state.Add(
                      order.Confirmation.Order,
                      new OrderViewModel(
                          order.Guest,
                          order.FoodOrder.Select(OrderViewModel.OrderItem.Create).ToList(),
                          order.DrinkOrder.Select(OrderViewModel.OrderItem.Create).ToList(),
                          order.Confirmation
                        )
                      ),
                 DeliveryReceived delivery => MarkAsDelivered(state, delivery),
                 _ => state
             });
        }

        public void OnGet()
        {

        }

        public IEnumerable<OrderViewModel> Orders { get => _orders.Values; }

    }


}
