using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Billing.Model;
using Api.GuestExperience.Model;
using Api.TableService.Model;

namespace Customer
{
    public abstract class Event
    {
        public DateTime On { get; set; } = DateTime.Now;
    }

    public interface IGuestEvent
    {
        int Guest { get; }
    }

    public class MenuRetrieved : Event, IGuestEvent
    {
        public Menu Menu { get; set; }
        public int Guest => Menu.Guest;
    }

    public class OrderPlaced : Event, IGuestEvent
    {
        public class OrderItem
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public List<string> Nutrition { get; set; }

            public decimal Price { get; set; }
        }

        public int Guest { get; set; }
        public OrderConfirmation Confirmation { get; set; }
        public List<OrderItem> FoodOrder { get; set; }
        public List<OrderItem> DrinkOrder { get; set; }
    }

    public class DeliveryReceived : Event, IGuestEvent
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

    public class BillReceived : Event, IGuestEvent
    {
        public BillReceived(int guest, int bill, List<int> orderedFood, List<int> orderedDrinks, decimal totalSum)
        {
            Guest = guest;
            Bill = bill;
            OrderedFood = orderedFood;
            OrderedDrinks = orderedDrinks;
            TotalSum = totalSum;
        }

        public int Guest { get; }
        public int Bill { get; }
        public List<int> OrderedFood { get; }
        public List<int> OrderedDrinks { get; }
        public decimal TotalSum { get; }
    }

    public class BillUpdated : Event, IGuestEvent
    {
        public BillUpdated(int guest, int bill, List<int> orderedFood, List<int> orderedDrinks, decimal totalSum)
        {
            Guest = guest;
            Bill = bill;
            OrderedFood = orderedFood;
            OrderedDrinks = orderedDrinks;
            TotalSum = totalSum;
        }

        public int Guest { get; }
        public int Bill { get; }
        public List<int> OrderedFood { get; }
        public List<int> OrderedDrinks { get; }
        public decimal TotalSum { get; }
    }

    public class PaymentMethodsSupported : Event, IGuestEvent
    {
        public PaymentMethodsSupported(int guest, int bill, PaymentMethod[] paymentMethods)
        {
            Guest = guest;
            Bill = bill;
            PaymentMethods = paymentMethods;
        }

        public int Guest { get; }
        public int Bill { get; }
        public PaymentMethod[] PaymentMethods { get; }
    }

    public class BillPaid : Event, IGuestEvent
    {
        public BillPaid(int guest, int bill, decimal amount, List<PaidOrder> paidOrders)
        {
            Guest = guest;
            Bill = bill;
            Amount = amount;
            PaidOrders = paidOrders;
        }

        public int Guest { get; }
        public int Bill { get; }
        public decimal Amount { get; }
        public List<PaidOrder> PaidOrders { get; }
    }
}