using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.GuestExperience.Model;
using Api.TableService.Model;

namespace Customer
{
    public abstract class Event
    {
        public DateTime On { get; set; } = DateTime.Now;
    }

    public class MenuRetrieved : Event
    {
        public Menu Menu { get; set; }
    }

    public class OrderPlaced : Event
    {
        public class OrderItem
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public List<string> Nutrition { get; set; }

            public decimal Price { get; set; }
        }

        public int Guest {get;set;}
        public OrderConfirmation Confirmation { get; set; }
        public List<OrderItem> FoodOrder { get; set; }
        public List<OrderItem> DrinkOrder { get; set; }
    }

    public class DeliveryReceived : Event
    {
        public DeliveryReceived(int delivery, int guest, int order, DeliveredItems items)
        {
            Delivery = delivery;
            Guest = guest;
            Order = order;
            Items = items;
        }

        public int Guest { get; }
        public int Order { get; }
        public DeliveredItems Items { get; }
        public int Delivery { get; }
    }
}