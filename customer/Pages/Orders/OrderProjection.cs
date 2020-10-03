using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Api.TableService.Model;

namespace Customer.Pages.Orders
{
    using GuestOrders = ImmutableDictionary<int, ImmutableDictionary<int, OrderViewModel>>;

    public class OrderViewModel
    {
        public OrderViewModel(int guest, DateTime orderTime, List<OrderItem> foodOrder, List<OrderItem> drinkOrder, OrderConfirmation confirmation)
        {
            Guest = guest;
            OrderTime = orderTime;
            FoodOrder = foodOrder;
            DrinkOrder = drinkOrder;
            Confirmation = confirmation;
        }

        public int Guest { get; }
        public DateTime OrderTime { get; }
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

        public TimeSpan? OrderDuration
        {
            get
            {
                var receivedAtForItems =
                    FoodOrder
                        .Concat(DrinkOrder)
                        .Select(x => x.ReceivedAt)
                        .ToList();

                if (receivedAtForItems.All(x => x != null))
                {
                    var minReceived = receivedAtForItems.Min(x => x.DeliveredOn);
                    var maxReceived = receivedAtForItems.Max(x => x.DeliveredOn);
                    var duration = maxReceived.Subtract(minReceived);

                    return duration;

                }
                else return null;
            }
        }

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

        private static GuestOrders AppendOrder(GuestOrders state, OrderPlaced order)
        {
            var orders = state
                .GetValueOrDefault(order.Guest, ImmutableDictionary<int, OrderViewModel>.Empty)
                .Add(
                    order.Confirmation.Order,
                    new OrderViewModel(
                        order.Guest,
                        order.On,
                        order.FoodOrder.Select(OrderViewModel.OrderItem.Create).ToList(),
                        order.DrinkOrder.Select(OrderViewModel.OrderItem.Create).ToList(),
                        order.Confirmation
                    )
                );

            return state.SetItem(order.Guest, orders);
        }

        public static GuestOrders Projection(GuestOrders state, Event @event) => @event switch
        {
            OrderPlaced order => AppendOrder(state, order),
            DeliveryReceived delivery =>
               state.SetItem(
                   delivery.Guest,
                   MarkAsDelivered(state.GetValueOrDefault(delivery.Guest), delivery)
               ),
            _ => state
        };
    }

    public class GuestOrderModel
    {
        public int Guest { get; set; }
        public IEnumerable<OrderViewModel> Orders { get; set; }
    }

}